﻿using MovieApp.DataAccess.Data;
using MovieApp.DataAccess.Repository.IRepository;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieApp.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            _db.OrderDetails.Update(orderDetail);
        }
    }
}
