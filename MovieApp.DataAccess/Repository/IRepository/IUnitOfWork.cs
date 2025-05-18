﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository categoryRepository { get; }

        IMovieRepository movieRepository { get; }
        IShoppingCartRepository shoppingCartRepository { get; }
        IApplicationUserRepository applicationUserRepository { get; }
        IOrderHeaderRepository orderHeaderRepository { get; }
        IOrderDetailRepository orderDetailRepository { get; }
        void Save();
    }
}
