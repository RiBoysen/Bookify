﻿using Bookify.Core;
using Bookify.Core.Interfaces;
using Bookify.Core.Interfaces.Repositories;
using Bookify.Models;

namespace Bookify.DataAccess.Repositories
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(BookifyContext ctx) : base(ctx)
        {

        }
    }
}
