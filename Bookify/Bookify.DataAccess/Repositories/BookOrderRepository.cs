﻿using Bookify.Core;
using Bookify.Core.Interfaces;
using Bookify.Models;

namespace Bookify.DataAccess.Repositories
{
    public class BookOrderRepository : GenericRepository<BookOrder>, IBookOrderRepository
    {
        public BookOrderRepository(BookifyContext ctx) : base(ctx)
        {

        }
    }
}