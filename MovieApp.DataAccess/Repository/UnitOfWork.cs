using MovieApp.DataAccess.Data;
using MovieApp.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository categoryRepository { get; private set; }
        public IMovieRepository movieRepository { get; private set; }
        public IShoppingCartRepository shoppingCartRepository { get; private set; }
        public IApplicationUserRepository applicationUserRepository { get; private set; }
        public IOrderHeaderRepository orderHeaderRepository { get; private set; }
        public IOrderDetailRepository orderDetailRepository { get; private set; }
        public IMovieVoteRepository movieVoteRepository { get; private set; } // Add this line

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            categoryRepository = new CategoryRepository(_db);
            movieRepository = new MovieRepository(_db);
            shoppingCartRepository = new ShoppingCartRepository(_db);
            applicationUserRepository = new ApplicationUserRepository(_db);
            orderHeaderRepository = new OrderHeaderRepository(_db);
            orderDetailRepository = new OrderDetailRepository(_db);
            movieVoteRepository = new MovieVoteRepository(_db); // Make sure this line exists
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}