using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using MovieApp.Models.ViewModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MovieAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: Review/Create
        public IActionResult Create(int? movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if movieId is provided and if user has already reviewed this movie
            if (movieId.HasValue)
            {
                var existingReview = _unitOfWork.Review.Get(r => r.MovieId == movieId.Value && r.UserId == userId);
                if (existingReview != null)
                {
                    TempData["error"] = "You have already reviewed this movie.";
                    return RedirectToAction("Details", "Movie", new { id = movieId.Value });
                }
            }

            var reviewVM = new ReviewVM
            {
                Review = new Review
                {
                    MovieId = movieId ?? 0,
                    UserId = userId,
                    CreatedDate = DateTime.Now
                },
                MovieList = GetMovieSelectList()
            };

            return View(reviewVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReviewVM reviewVM)
        {
            // Add this at the very top for debugging
            System.Diagnostics.Debug.WriteLine("POST Create action hit!");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            reviewVM.Review.UserId = userId;
            reviewVM.Review.CreatedDate = DateTime.Now;

            // Check if user has already reviewed this movie
            var existingReview = _unitOfWork.Review.Get(r => r.MovieId == reviewVM.Review.MovieId && r.UserId == userId);
            if (existingReview != null)
            {
                TempData["error"] = "You have already reviewed this movie.";
                return RedirectToAction("Details", "Movie", new { id = reviewVM.Review.MovieId });
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Review.Add(reviewVM.Review);
                _unitOfWork.Save();
                TempData["success"] = "Review created successfully!";
                return RedirectToAction("Details", "Movie", new { id = reviewVM.Review.MovieId });
            }

            // Add debugging for ModelState errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                System.Diagnostics.Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
            }

            // Reload dropdown if validation fails
            reviewVM.MovieList = GetMovieSelectList();
            return View(reviewVM);
        }

        // GET: Review/Edit/5
        public IActionResult Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.Review.Get(r => r.ReviewId == id && r.UserId == userId, includeProperties: "Movie");

            if (review == null)
            {
                TempData["error"] = "Review not found or you don't have permission to edit this review.";
                return RedirectToAction("Index", "Home");
            }

            var reviewVM = new ReviewVM
            {
                Review = review,
                MovieList = GetMovieSelectList()
            };

            return View(reviewVM);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ReviewVM reviewVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingReview = _unitOfWork.Review.Get(r => r.ReviewId == reviewVM.Review.ReviewId && r.UserId == userId);

            if (existingReview == null)
            {
                TempData["error"] = "Review not found or you don't have permission to edit this review.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                existingReview.Rating = reviewVM.Review.Rating;
                existingReview.Title = reviewVM.Review.Title;
                existingReview.Content = reviewVM.Review.Content;
                existingReview.UpdatedDate = DateTime.Now;

                // Note: No explicit Update call needed if using Entity Framework change tracking
                _unitOfWork.Save();
                TempData["success"] = "Review updated successfully!";
                return RedirectToAction("Details", "Movie", new { id = existingReview.MovieId });
            }

            reviewVM.MovieList = GetMovieSelectList();
            return View(reviewVM);
        }

        // GET: Review/Delete/5
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.Review.Get(r => r.ReviewId == id && r.UserId == userId, includeProperties: "Movie");

            if (review == null)
            {
                TempData["error"] = "Review not found or you don't have permission to delete this review.";
                return RedirectToAction("Index", "Home");
            }

            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.Review.Get(r => r.ReviewId == id && r.UserId == userId);

            if (review == null)
            {
                TempData["error"] = "Review not found or you don't have permission to delete this review.";
                return RedirectToAction("Index", "Home");
            }

            var movieId = review.MovieId;
            _unitOfWork.Review.Remove(review);
            _unitOfWork.Save();
            TempData["success"] = "Review deleted successfully!";
            return RedirectToAction("Details", "Movie", new { id = movieId });
        }

        // GET: Review/MyReviews
        public IActionResult MyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviews = _unitOfWork.Review.GetAll(r => r.UserId == userId, includeProperties: "Movie");
            return View(reviews);
        }

        private IEnumerable<SelectListItem> GetMovieSelectList()
        {
            var movies = _unitOfWork.movieRepository.GetAll();
            return movies.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(), 
                Text = m.Name 
            });
        }
    }
}