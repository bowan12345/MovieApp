using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models.ViewModels;
using MovieApp.Models;
using MovieApp.Utility;
using Stripe;
using System.Security.Claims;
using Stripe.Checkout;

namespace MovieAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            //check all order records
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(RoleName.Role_Admin))
            {
                orderHeaders = _unitOfWork.orderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                //customer, only check by userId
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaders = _unitOfWork.orderHeaderRepository.GetAll(x => x.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == OrderStatus.PaymentStatusPending);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == OrderStatus.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeaders });

        }


        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.orderHeaderRepository.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.orderDetailRepository.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Movie")
            };
            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult PayNow(OrderHeader orderHeader)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.orderHeaderRepository.Get(u => u.Id == orderHeader.Id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.orderDetailRepository.GetAll(u => u.OrderHeaderId == orderHeader.Id, includeProperties: "Movie")
            };

            //  Azure domain  
            //var domain = "https://movieappweb20250531114550.azurewebsites.net/";
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Customer/Cart/Orderconfirmation?id={orderHeader.Id}",
                CancelUrl = domain + $"Admin/Order/Details?orderId={orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetails)
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
            _unitOfWork.orderHeaderRepository.UpdateStripePaymentIDById(orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }


        [HttpPost]
        [Authorize(Roles = RoleName.Role_Admin)]
        public IActionResult UpdateOrderDetail()
        {
            //update booker's details only for name right now
            var orderHeaderFromDb = _unitOfWork.orderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            _unitOfWork.orderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });

        }


        [HttpPost]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.orderHeaderRepository.Get(u => u.Id == OrderVM.OrderHeader.Id);

            //after paid, make refund and update orders status
            if (orderHeader.PaymentStatus == OrderStatus.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.orderHeaderRepository.UpdateStatus(orderHeader.Id, OrderStatus.StatusCancelled, OrderStatus.StatusRefunded);
            }
            else
            {
                //pending, just update order status
                _unitOfWork.orderHeaderRepository.UpdateStatus(orderHeader.Id, OrderStatus.StatusCancelled, OrderStatus.StatusCancelled);
            }

            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


    }
}
