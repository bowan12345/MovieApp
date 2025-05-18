using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models.ViewModels;
using MovieApp.Models;
using Stripe.Checkout;
using System.Security.Claims;
using MovieApp.Utility;

namespace MovieAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }


        public CartController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //get login user info
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            //get userId
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitOfWork.shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId,
                includeProperties: "Movie"),
                OrderHeader = new()
            };


            foreach (var shoppingCart in ShoppingCartVM.ShoppingCartList)
            {
                shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
                //check if price is valid
                if (shoppingCart.Price < 0)
                {
                    TempData["error"] = "Shopping Cart is invalid. Please remove the item(s) and try again.";
                    return RedirectToAction(nameof(Index));
                }
                ShoppingCartVM.OrderHeader.OrderTotal += (shoppingCart.Count * shoppingCart.Price);
                //shoppingCartVM.OrderTotal += (shoppingCart.Count*shoppingCart.Price);
            }


            return View(ShoppingCartVM);
        }


        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCartRepository.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.shoppingCartRepository.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCartRepository.Get(u => u.Id == cartId);
            cartFromDb.Count -= 1;
            //check if count is less than 0, if yes, remove the item from cart
            if (cartFromDb.Count == 0)
            {
                _unitOfWork.shoppingCartRepository.Remove(cartFromDb);
            }
            else
            {
                _unitOfWork.shoppingCartRepository.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.shoppingCartRepository.Get(u => u.Id == cartId);
            _unitOfWork.shoppingCartRepository.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // go to the summary page
        public IActionResult Summary()
        {

            //get login user info
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //create a shoppinglist return obj
            ShoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitOfWork.shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId,
                includeProperties: "Movie"),
                OrderHeader = new()
            };

            // assemble user address info
            ApplicationUser applicationUser = _unitOfWork.applicationUserRepository.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUser;
            ShoppingCartVM.OrderHeader.Name = applicationUser.Name;
            //calculate the total price
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);

        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {

            //get login user info
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //create a shoppinglist return obj
            ShoppingCartVM.ShoppingCartList = _unitOfWork.shoppingCartRepository.GetAll(u => u.ApplicationUserId == userId,
            includeProperties: "Movie");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;


            //calculate the total price
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            // assemble user address info
            ApplicationUser applicationUser = _unitOfWork.applicationUserRepository.Get(u => u.Id == userId);
            
            //customer
            ShoppingCartVM.OrderHeader.PaymentStatus = OrderStatus.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            

            //save orderheader to database
            _unitOfWork.orderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            //save order details to the database
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    MovieId = cart.MovieId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };

                _unitOfWork.orderDetailRepository.Add(orderDetail);
                _unitOfWork.Save();
            }

            //customer
            var domain = "https://localhost:7194/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Customer/Cart/Orderconfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"Customer/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "nzd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            // update orderheader to database
            _unitOfWork.orderHeaderRepository.UpdateStripePaymentIDById(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
            //return View(ShoppingCartVM);

        }


        //after payment successfully, recall to update payment status
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.orderHeaderRepository.Get(x => x.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != OrderStatus.PaymentStatusDelayedPayment)
            {
                //customer
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.orderHeaderRepository.UpdateStripePaymentIDById(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.orderHeaderRepository.UpdateStatus(id, OrderStatus.StatusApproved, OrderStatus.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                //HttpContext.Session.Clear();
            }

            //remove shopping cart
            List<ShoppingCart> shoppingCarts = _unitOfWork.shoppingCartRepository.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.shoppingCartRepository.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);



        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {

            if (shoppingCart.Count > 0 && shoppingCart.Count <= 5)
            {
                shoppingCart.Price = shoppingCart.Movie.Price;
            }
            else if (shoppingCart.Count > 6 && shoppingCart.Count <= 10)
            {
                shoppingCart.Price = shoppingCart.Movie.Price5;
            }
            else if (shoppingCart.Count > 10)
            {
                shoppingCart.Price = shoppingCart.Movie.Price10;
            }
            else
            {
                shoppingCart.Price = -1;
            }
            return shoppingCart.Price;
        }
    }
}
