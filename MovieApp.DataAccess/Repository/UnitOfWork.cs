using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public ICategoryRepository categoryRepository { get; private set; }
        public IMovieRepository movieRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            categoryRepository = new CategoryRepository(_db);
            movieRepository = new MovieRepository(_db);
        }

        public void Save() 
        {
            _db.SaveChanges();
        }

    }
}
