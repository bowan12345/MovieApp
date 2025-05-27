using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie;
using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using MovieApp.Models.ViewModels;
using MovieApp.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace MovieAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string searchTerm)
        {
            /*IEnumerable<Movie> movieList = _unitOfWork.movieRepository.GetAll(includeProperties: "Category");
            return View(movieList);*/
            // Store the search term in ViewBag for display in the view
            ViewBag.CurrentSearchTerm = searchTerm;

            // Create a query that we can build upon
            IEnumerable<Movie> movieList = _unitOfWork.movieRepository.GetAll(includeProperties: "Category");

            // Apply search filter if a search term is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Search by movie name 
                movieList = movieList.Where(m => m.Name.ToLower().Contains(searchTerm.ToLower()) ||
                                        (m.Director!= null && m.Director.ToLower().Contains(searchTerm.ToLower())) ||
                                        (m.Description!= null && m.Description.ToLower().Contains(searchTerm.ToLower())));
            }
            
            return View(movieList);
        }


        public IActionResult Details(int movieId)
        {
            ShoppingCart cart = new()
            {

                Movie = _unitOfWork.movieRepository.Get(obj => obj.Id == movieId, includeProperties: "Category"),
                Count = 1,
                MovieId = movieId
            };

            return View(cart);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
			//verify count 
			if (cart.Count <= 0)
			{
				TempData["error"] = "Please select at least one ticket.";
				// retrive movie data from db
				cart.Movie = _unitOfWork.movieRepository.Get(
					m => m.Id == cart.MovieId,
					includeProperties: "Category"
				);
				return View(cart);
			}

			//get login user info
			var claimsIdentity = (ClaimsIdentity)User.Identity;
            //get userId
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            cart.ApplicationUserId = userId;

            ShoppingCart shoppingCartFromDb = _unitOfWork.shoppingCartRepository.Get(x => x.ApplicationUserId == userId && x.MovieId == cart.MovieId);
			
			if (shoppingCartFromDb == null)
            {
                //add new one
                _unitOfWork.shoppingCartRepository.Add(cart);
                _unitOfWork.Save();
                //add shopping cart count into session
                HttpContext.Session.SetInt32(SessionConstants.SessionCart, _unitOfWork.shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId).Count());
            }
            else
            {
                //update only increase the count
                shoppingCartFromDb.Count += cart.Count;
                _unitOfWork.shoppingCartRepository.Update(shoppingCartFromDb);
                _unitOfWork.Save();
            }
            TempData["success"] = "Shopping Cart Updated Successfully!!!";
            return RedirectToAction("Index");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}