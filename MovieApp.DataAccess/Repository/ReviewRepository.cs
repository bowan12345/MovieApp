using Microsoft.EntityFrameworkCore;
using MovieApp.DataAccess.Data;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.DataAccess.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _db;

        public ReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Review>> GetReviewsByMovieAsync(int movieId)
        {
            return await _db.Reviews
                .Include(r => r.ApplicationUser)
                .Include(r => r.Movie)
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId)
        {
            return await _db.Reviews
                .Include(r => r.Movie)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<Review> GetUserReviewForMovieAsync(int movieId, string userId)
        {
            return await _db.Reviews
                .Include(r => r.Movie)
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId);
        }

        public async Task<double> GetAverageRatingForMovieAsync(int movieId)
        {
            var reviews = await _db.Reviews
                .Where(r => r.MovieId == movieId)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<int> GetReviewCountForMovieAsync(int movieId)
        {
            return await _db.Reviews.CountAsync(r => r.MovieId == movieId);
        }

        public async Task<bool> HasUserReviewedMovieAsync(int movieId, string userId)
        {
            return await _db.Reviews.AnyAsync(r => r.MovieId == movieId && r.UserId == userId);
        }
    }
}
