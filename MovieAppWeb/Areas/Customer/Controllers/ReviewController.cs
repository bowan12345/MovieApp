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
            var existingReview = _unitOfWork.reviewRepository.Get(r => r.MovieId == movieId && r.UserId == userId);
            if (existingReview != null)
            {
                TempData["error"] = "You have already reviewed this movie.";
                return RedirectToAction("MyReviews", "Review");
            }

            var reviewVM = new ReviewVM
            {
                Review = new Review
                {
                    MovieId = movieId ?? 0,
                    UserId = userId,
                    CreatedDate = DateTime.Now
                },
            };

            return View(reviewVM);
        }

        [HttpPost]
        public IActionResult Create(ReviewVM reviewVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            reviewVM.Review.UserId = userId;
            reviewVM.Review.CreatedDate = DateTime.Now;

            // Check if user has already reviewed this movie
            var existingReview = _unitOfWork.reviewRepository.Get(r => r.MovieId == reviewVM.Review.MovieId && r.UserId == userId);
            if (existingReview != null)
            {
                TempData["error"] = "You have already reviewed this movie.";
                return RedirectToAction("Details", "Movie", new { id = reviewVM.Review.MovieId });
            }

            // add a new review
            _unitOfWork.reviewRepository.Add(reviewVM.Review);
            _unitOfWork.Save();
            TempData["success"] = "Review created successfully!";
            return RedirectToAction("Details", "Home", new { movieId = reviewVM.Review.MovieId });
           
        }

        // GET: Review/Edit/5
        public IActionResult Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.reviewRepository.Get(r => r.ReviewId == id && r.UserId == userId, includeProperties: "Movie");

            if (review == null)
            {
                TempData["error"] = "Review not found or you don't have permission to edit this review.";
                return RedirectToAction("MyReviews", "Review");
            }

            var reviewVM = new ReviewVM
            {
                Review = review,
            };

            return View(reviewVM);
        }

        // POST: Review/Edit/5
        [HttpPost]
        public IActionResult Edit(ReviewVM reviewVM)
        {
            //get user info
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingReview = _unitOfWork.reviewRepository.Get(r => r.ReviewId == reviewVM.Review.ReviewId && r.UserId == userId);

            if (existingReview == null)
            {
                TempData["error"] = "Review not found or you don't have permission to edit this review.";
                return RedirectToAction("MyReviews", "Review");
            }
            existingReview.Rating = reviewVM.Review.Rating;
            existingReview.Title = reviewVM.Review.Title;
            existingReview.Content = reviewVM.Review.Content;
            existingReview.UpdatedDate = DateTime.Now;

            // update
            _unitOfWork.Save();
            TempData["success"] = "Review updated successfully!";
            return RedirectToAction("MyReviews", "Review");


        }

        // GET: Review/Delete/5
        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.reviewRepository.Get(r => r.ReviewId == id && r.UserId == userId, includeProperties: "Movie");

            if (review == null)
            {
                TempData["error"] = "Review not found or you don't have permission to delete this review.";
                return RedirectToAction("MyReviews", "Review");
            }

            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = _unitOfWork.reviewRepository.Get(r => r.ReviewId == id && r.UserId == userId);

            if (review == null)
            {
                TempData["error"] = "Review not found or you don't have permission to delete this review.";
                return RedirectToAction("MyReviews", "Review");
            }

            var movieId = review.MovieId;
            _unitOfWork.reviewRepository.Remove(review);
            _unitOfWork.Save();
            TempData["success"] = "Review deleted successfully!";
            return RedirectToAction("MyReviews", "Review");
        }


        // GET: Review/All
        public IActionResult AllReview(int movieId)
        {
            // Get the movie details
            var movie = _unitOfWork.movieRepository.Get(m => m.Id == movieId);
            if (movie == null)
            {
                return NotFound();
            }

            // Get all reviews for this movie
            var reviews = _unitOfWork.reviewRepository.GetAll(
                r => r.MovieId == movieId,
                includeProperties: "ApplicationUser")
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            // Pass the movie and reviews to the view
            ViewBag.Movie = movie;
            ViewBag.Reviews = reviews;

            return View();
        }


        // GET: Review/MyReviews
        public IActionResult MyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reviews = _unitOfWork.reviewRepository.GetAll(r => r.UserId == userId, includeProperties: "Movie");
            return View(reviews);
        }

    }
}