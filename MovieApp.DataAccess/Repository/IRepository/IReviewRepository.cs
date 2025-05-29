using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieApp.Models;

namespace MovieApp.DataAccess.Repository.IRepository
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByMovieAsync(int movieId);
        Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId);
        Task<Review> GetUserReviewForMovieAsync(int movieId, string userId);
        Task<double> GetAverageRatingForMovieAsync(int movieId);
        Task<int> GetReviewCountForMovieAsync(int movieId);
        Task<bool> HasUserReviewedMovieAsync(int movieId, string userId);
    }
}
