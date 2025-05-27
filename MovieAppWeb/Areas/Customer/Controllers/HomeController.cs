using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie;
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
            // Store the search term in ViewBag for display in the view
            ViewBag.CurrentSearchTerm = searchTerm;

            // Create a query that we can build upon
            IEnumerable<Movie> movieList = _unitOfWork.movieRepository.GetAll(includeProperties: "Category");

            // Apply search filter if a search term is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Search by movie name 
                movieList = movieList.Where(m => m.Name.ToLower().Contains(searchTerm.ToLower()) ||
                                        (m.Director != null && m.Director.ToLower().Contains(searchTerm.ToLower())) ||
                                        (m.Description != null && m.Description.ToLower().Contains(searchTerm.ToLower())));
            }

            return View(movieList);
        }

        public IActionResult Details(int movieId)
        {
            var movie = _unitOfWork.movieRepository.Get(obj => obj.Id == movieId, includeProperties: "Category");

            if (movie == null)
            {
                return NotFound();
            }

            // Get vote counts for this movie
            var voteCounts = _unitOfWork.movieVoteRepository.GetMovieVoteCounts(movieId);
            movie.LikeCount = voteCounts.likeCount;
            movie.DislikeCount = voteCounts.dislikeCount;

            ShoppingCart cart = new()
            {
                Movie = movie,
                Count = 1,
                MovieId = movieId
            };

            // Check if current user has voted (for authenticated users)
            if (User.Identity.IsAuthenticated)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId != null)
                {
                    var userVote = _unitOfWork.movieVoteRepository.GetUserVoteForMovie(movieId, userId);
                    // null if no vote, true if like, false if dislike
                    if (userVote == null)
                    {
                        ViewBag.UserVote = "none"; 
                    }
                    else if (userVote.IsLike)
                    {
                        ViewBag.UserVote = "like";
                    }
                    else
                    {
                        ViewBag.UserVote = "dislike";
                    } 
                }
            }

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
                // retrieve movie data from db
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult LikeMovie(int movieId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                // Check if user already voted for this movie
                var existingVote = _unitOfWork.movieVoteRepository.GetUserVoteForMovie(movieId, userId);

                if (existingVote != null)
                {
                    if (existingVote.IsLike == true)
                    {
                        // User already liked, remove the like
                        _unitOfWork.movieVoteRepository.Remove(existingVote);
                        _unitOfWork.Save();

                        var voteCounts = _unitOfWork.movieVoteRepository.GetMovieVoteCounts(movieId);
                        return Json(new
                        {
                            success = true,
                            message = "Like removed",
                            likeCount = voteCounts.likeCount,
                            dislikeCount = voteCounts.dislikeCount,
                            userVote = (bool?)null
                        });
                    }
                    else
                    {
                        // User previously disliked, change to like
                        existingVote.IsLike = true;
                        existingVote.VotedAt = DateTime.Now;
                        _unitOfWork.movieVoteRepository.Update(existingVote);
                        _unitOfWork.Save();

                        var voteCounts = _unitOfWork.movieVoteRepository.GetMovieVoteCounts(movieId);
                        return Json(new
                        {
                            success = true,
                            message = "Changed to like",
                            likeCount = voteCounts.likeCount,
                            dislikeCount = voteCounts.dislikeCount,
                            userVote = true
                        });
                    }
                }
                else
                {
                    // No existing vote, create new like
                    var newVote = new MovieVote
                    {
                        MovieId = movieId,
                        UserId = userId,
                        IsLike = true,
                        VotedAt = DateTime.Now
                    };

                    _unitOfWork.movieVoteRepository.Add(newVote);
                    _unitOfWork.Save();

                    var voteCounts = _unitOfWork.movieVoteRepository.GetMovieVoteCounts(movieId);
                    return Json(new
                    {
                        success = true,
                        message = "Movie liked",
                        likeCount = voteCounts.likeCount,
                        dislikeCount = voteCounts.dislikeCount,
                        userVote = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing like vote for movie {MovieId} by user {UserId}", movieId, userId);
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DislikeMovie(int movieId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                // Check if user already voted for this movie
                var existingVote = _unitOfWork.movieVoteRepository.GetUserVoteForMovie(movieId, userId);

                if (existingVote != null)
                {
                    if (existingVote.IsLike == false)
                    {
                        // User already disliked, remove the dislike
                        _unitOfWork.movieVoteRepository.Remove(existingVote);
                        _unitOfWork.Save();

                        var voteCounts = _unitOfWork.movieVoteRepository.GetMovieVoteCounts(movieId);
                        return Json(new
                        {
                            success = true,
                            message = "Dislike removed",
                            likeCount = voteCounts.likeCount,
                            dislikeCount = voteCounts.dislikeCount,
                            userVote = (bool?)null
                        });
                    }
                    else
                    {
                        // User previously liked, change to dislike
                        existingVote.IsLike = false;
                        existingVote.VotedAt = DateTime.Now;
                        _unitOfWork.movieVoteRepository.Update(existingVote);
                        _unitOfWork.Save();

                        var voteCounts = _unitOfWork.movieVoteRepository.GetMovieVoteCounts(movieId);
                        return Json(new
                        {
                            success = true,
                            message = "Changed to dislike",
                            likeCount = voteCounts.likeCount,
                            dislikeCount = voteCounts.dislikeCount,
                            userVote = false
                        });
                    }
                }
                else
                {
                    // No existing vote, create new dislike
                    var newVote = new MovieVote
                    {
                        MovieId = movieId,
                        UserId = userId,
                        IsLike = false,
                        VotedAt = DateTime.Now
                    };

                    _unitOfWork.movieVoteRepository.Add(newVote);
                    _unitOfWork.Save();

                    var voteCounts = _unitOfWork.movieVoteRepository.GetMovieVoteCounts(movieId);
                    return Json(new
                    {
                        success = true,
                        message = "Movie disliked",
                        likeCount = voteCounts.likeCount,
                        dislikeCount = voteCounts.dislikeCount,
                        userVote = false
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing your vote" });
            }
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