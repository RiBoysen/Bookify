using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Bookify.App.Core.Models;
using Bookify.Common.Models;

namespace Bookify.App.Core.Interfaces.Services
{
    /// <summary>
    /// The Shopping Cart interface.
    /// </summary>
    public interface IShoppingCartService
    {
        /// <summary>
        /// Gets the books that currently reside in the cart.
        /// </summary>
        /// <value>
        /// The books.
        /// </value>
        ObservableCollection<CartItemModel> CartItems { get; }

        /// <summary>
        /// Adds the specified book to the cart.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        Task AddToCart(BookDto book);

        /// <summary>
        /// Removes the book from the cart.
        /// </summary>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        Task RemoveFromCart(BookDto book);

        /// <summary>
        /// Removes the book with the specified ID from the cart.
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns></returns>
        Task RemoveFromCart(int bookId);
    }
}