using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using MovieApp.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MovieAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleName.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categorys = _unitOfWork.categoryRepository.GetAll().ToList();
            return View(categorys);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> categorys = _unitOfWork.categoryRepository.GetAll().ToList();
            return Json(new { data = categorys });

        }


        //return a create/update page
        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                //create
                return View(new Category());
            }
            else
            {
                //update
                Category category = _unitOfWork.categoryRepository.Get(u => u.Id == id);
                return View(category);
            }
        }

        [HttpPost]
        public IActionResult Create(Category _category)
        {

            if (ModelState.IsValid)
            {
                //create
                if (_category.Id == 0)
                {
                    //check the duplicated name
                    Category category = _unitOfWork.categoryRepository.Get(u => u.Name == _category.Name);
                    if (category != null)
                    {
                        TempData["error"] = "Category Name Already Exist!!!";
                        return View("Upsert", _category);
                    }
                    _unitOfWork.categoryRepository.Add(_category);
                    TempData["success"] = "Category created successfully";
                }
                else
                {
                    //update
                    _unitOfWork.categoryRepository.Update(_category);
                    TempData["success"] = "Category updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(_category);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            Category category = _unitOfWork.categoryRepository.Get(u => u.Id == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Category not found" });
            }
            _unitOfWork.categoryRepository.Remove(category);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successfully" });
        }
    }
}