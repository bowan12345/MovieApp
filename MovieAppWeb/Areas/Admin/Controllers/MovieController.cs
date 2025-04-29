using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using MovieApp.Models.ViewModels;

namespace MovieAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MovieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Movie> movies = _unitOfWork.movieRepository.GetAll(includeProperties: "Category").ToList();
            return View(movies);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Movie> movies = _unitOfWork.movieRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = movies });

        }


        //return a create page
        public IActionResult Upsert(int? id)
        {
            MovieVM movieVM = new()
            {
                CategoryList = _unitOfWork.categoryRepository.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Movie = new Movie()
            };
            if (id == null || id == 0)
            {
                //create
                return View(movieVM);
            }
            else
            {
                //update
                movieVM.Movie = _unitOfWork.movieRepository.Get(u => u.Id == id);
                return View(movieVM);
            }


        }

        [HttpPost]
        public IActionResult Create(MovieVM _Movie, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");//C:\\document\\CSharp\\Bulky\\BulkyWeb\\wwwroot\\images\\product"

                    //check imageUrl is null or not
                    if (!string.IsNullOrEmpty(_Movie.Movie.ImageUrl))
                    {
                        //delete old image
                        string oldImage = Path.Combine(wwwRootPath, _Movie.Movie.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    _Movie.Movie.ImageUrl = @"\images\product\" + fileName;
                }

                if (_Movie.Movie.Id == 0)
                {
                    _unitOfWork.movieRepository.Add(_Movie.Movie);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.movieRepository.Update(_Movie.Movie);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                _Movie.CategoryList = _unitOfWork.categoryRepository.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(_Movie);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            Movie movie = _unitOfWork.movieRepository.Get(u => u.Id == id);
            if (movie == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }
            //delete old image
            string oldImage = Path.Combine(_webHostEnvironment.WebRootPath, movie.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            _unitOfWork.movieRepository.Remove(movie);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successfully" });
        }
    }
}
