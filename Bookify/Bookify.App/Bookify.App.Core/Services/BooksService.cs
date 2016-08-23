using System.Threading.Tasks;

using Bookify.App.Core.Interfaces.Services;
using Bookify.App.Sdk.Interfaces;
using Bookify.Common.Filter;
using Bookify.Common.Models;

namespace Bookify.App.Core.Services
{
    /// <summary>
    /// The Book Service implementation
    /// </summary>
    /// <seealso cref="Bookify.App.Core.Interfaces.Services.IBooksService" />
    public class BooksService : IBooksService
    {
        /// <summary>
        /// The API
        /// </summary>
        private readonly IBooksApi api;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksService"/> class.
        /// </summary>
        /// <param name="api">The API.</param>
        public BooksService(IBooksApi api)
        {
            this.api = api;
        }

        /// <summary>
        /// Gets the book with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<DetailedBookDto> GetBook(int id)
        {
            return await this.api.Get(id);
        }

        /// <summary>
        /// Gets the items, using the provided Filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public async Task<IPaginatedEnumerable<BookDto>> GetItems(BookFilter filter)
        {
            return await this.api.GetBooks(filter);
        }
    }
}