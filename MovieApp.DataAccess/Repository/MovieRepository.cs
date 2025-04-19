using MovieApp.DataAccess.Data;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataAccess.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private ApplicationDbContext _db;
        public MovieRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

 /*       void IProductRepository.Save()
        {
            _db.SaveChanges();
        }*/

        public void Update(Movie movie)
        {
            var moviefromDb = _db.Movies.FirstOrDefault(p => p.Id == movie.Id);
            if (moviefromDb != null)
            {
                moviefromDb.Title = movie.Title;
                moviefromDb.Director = movie.Director;
                moviefromDb.Price = movie.Price;
                moviefromDb.ListPrice = movie.ListPrice;
                moviefromDb.Price50 = movie.Price50;
                moviefromDb.Price100 = movie.Price100;
                moviefromDb.Duration = movie.Duration;
                moviefromDb.Description = movie.Description;
                moviefromDb.CategoryId = movie.CategoryId;
                if (movie.ImageUrl != null)
                {
                    moviefromDb.ImageUrl = movie.ImageUrl;
                }

                _db.Update(moviefromDb);
            }
        }
    }
}
