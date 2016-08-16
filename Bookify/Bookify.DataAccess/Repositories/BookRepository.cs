﻿
using System;
using System.Threading.Tasks;
using Bookify.Models;
using System.Linq;
using System.Data.Entity;
using Bookify.Core.Extensions;
using Bookify.Core.Interfaces;

namespace Bookify.DataAccess.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(BookifyContext ctx) : base(ctx)
        {

        }

        public override async Task<Book> Find(int id)
        {
            return await _ctx.Books.Where(b => b.Id == id).Include(b => b.Genres).SingleAsync();
        }

        public async Task<IQueryable<Book>> GetAllByParams(int? skip, int? take, int[] genres, string search, string orderBy, bool? desc)
        {
            IQueryable<Book> result = await GetAll();

            /*
            if (genres != null && genres.Any())
            {
                result = result.Where(b => b.Genres.SelectMany(m => m.Id == genres.First()));
            }
            */

            if (!string.IsNullOrEmpty(search))
                result = result
                    .Where(b =>
                            string.Equals(b.Author.Name, search, StringComparison.CurrentCultureIgnoreCase) || 
                            b.ISBN == search ||
                            string.Equals(b.Publisher.Name, search, StringComparison.CurrentCultureIgnoreCase) || 
                            string.Equals(b.Title, search, StringComparison.CurrentCultureIgnoreCase));
            // string.Equals (a,b, StringComparison.CurrentCultureIgnoreCase) mean a compare a & b and ignore the case of the string

            if (orderBy != null)
                result = result.OrderBy(orderBy, desc);
            // Order by is a Extension -> doens't need to call the object it lays in to exec the method ;)

            if (skip != null)
                result = result.Skip(skip.Value);
            if (take != null)
                result = result.Take(take.Value);
            
            return result;
        }

      
        
    }
}