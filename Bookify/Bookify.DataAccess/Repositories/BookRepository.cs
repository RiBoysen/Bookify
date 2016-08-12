﻿using Bookify.Core;
using Bookify.Models;

namespace Bookify.DataAccess
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(BookifyContext ctx) : base(ctx)
        {

        }
    }
}