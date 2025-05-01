using Microsoft.AspNetCore.Mvc;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using System.Diagnostics;

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
        public IActionResult Index()
        {
            IEnumerable<Movie> movieList = _unitOfWork.movieRepository.GetAll(includeProperties: "Category");
            return View(movieList);
        }


        public IActionResult Details(int movieId)
        {
            Movie movie = _unitOfWork.movieRepository.Get(obj => obj.Id == movieId, includeProperties: "Category");
            return View(movie);
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