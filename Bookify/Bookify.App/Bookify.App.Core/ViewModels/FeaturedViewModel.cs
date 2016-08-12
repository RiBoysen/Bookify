using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Bookify.App.Core.Interfaces.Services;
using Bookify.App.Core.Models;

using Polly;

namespace Bookify.App.Core.ViewModels
{
    public class FeaturedViewModel : BaseViewModel
    {
        private readonly IBookService bookService;

        public FeaturedViewModel(IBookService bookService)
        {
            this.bookService = bookService;
        }

        public ObservableCollection<LightBookModel> Books { get; } = new ObservableCollection<LightBookModel>();

        public async Task<IEnumerable<LightBookModel>> LoadBooks(int index, int count)
        {
            var result = await
                         Policy.Handle<WebException>()
                             .RetryAsync()
                             .ExecuteAndCaptureAsync(async () => await this.bookService.GetBooks(index, count));
            if (result.Outcome == OutcomeType.Failure)
            {
                return null;
            }
            var newBooks = result.Result.ToArray();
            foreach (var book in newBooks)
            {
                this.Books.Add(book);
            }
            return newBooks;
        }

        public async Task<BookModel> GetBook(int id)
        {
            var result = await
                         Policy.Handle<WebException>()
                             .RetryAsync()
                             .ExecuteAndCaptureAsync(async () => await this.bookService.GetBook(id));
            if (result.Outcome == OutcomeType.Failure)
            {
                return null;
            }
            return result.Result;
        }
    }
}