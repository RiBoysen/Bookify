﻿using Bookify.Core;
using Bookify.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Bookify.Core.Interfaces;

namespace Bookify.API.Controllers
{
    public abstract class BaseFilter
    {
        public int Index { get; set; }
        public int Count { get; set; } = 10;
    }

    public class BookFilter : BaseFilter
    {
        public int[] GenreIds { get; set; }

        public string Search { get; set; }
    }

    public class BooksController : ApiController
    {
        private IBookRepository _bookRepo;
        private IBookHistoryRepository _bookHistoryRepo;
        private IPersonRepository _personRepo;
        private IBookOrderRepository _bookOrderRepo;
        private IBookContentRepository _bookContentRepo;
        private IBookFeedbackRepository _bookFeedbackRepo;

        public BooksController(IBookRepository bookRepo, IBookHistoryRepository bookHistoryRepo, IPersonRepository personRepo, IBookOrderRepository bookOrderRepo, IBookContentRepository bookContentRepo, IBookFeedbackRepository bookFeedbackRepo)
        {
            _bookRepo = bookRepo;
            _bookHistoryRepo = bookHistoryRepo;
            _personRepo = personRepo;
            _bookOrderRepo = bookOrderRepo;
            _bookContentRepo = bookContentRepo;
            _bookFeedbackRepo = bookFeedbackRepo;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri]BookFilter filter)
        {
            // Genres not added yet, other params works though
            var booksQuery = await _bookRepo.GetAllByParams(filter.Index, filter.Count, filter.GenreIds, filter.Search, null, false);
            // search for the books
            var books = booksQuery?.ToList() ?? new List<Book>();
            return Ok(books);
            
            // return the book when done searching 
        }

        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> Create(Book book)
        {
            var createdBook = await _bookRepo.Add(book);
            return Json(createdBook);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> Update(Book book)
        {
            await _bookRepo.Update(book);
            return Ok();
        }

        //[HttpPost]
        //public async Task<IHttpActionResult> Get(int id)
        //{
        //    /*
        //    Book book = null;
        //    try
        //    {
        //        book = await _bookRepo.Find(id);
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return NotFound();
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }
        //    return Ok(book);
        //    */
        //    return await CatchExceptions(async () => await _bookRepo.Find(id));
        //}


        [HttpDelete]
        [Authorize]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var bookHistory = new BookHistory()
            {
                BookId = id,
                Type = BookHistoryType.Deleted,
                Created = DateTime.Now
            };
            await _bookHistoryRepo.Add(bookHistory);
            await _bookHistoryRepo.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> History(int id)
        {
            var book = await _bookRepo.Find(id);
            return Ok(book.History);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Buy(int id, string email)
        {
            var person = _personRepo.CreatePersonIfNotExists(email);


            await _bookOrderRepo.Add(new BookOrder()
            {
                BookId = id,
                Created = DateTime.Now,
                Status = BookOrderStatus.Sold,
                PersonId = person.Id
            });

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> Download(int id)
        {
            var book = await _bookContentRepo.Find(id);
            return Content(HttpStatusCode.OK, book.Epub);
        }

        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> Review(int id, int personId, int rating, string text)
        {
            await _bookFeedbackRepo.Add(new BookFeedback()
            {
                BookId = id,
                PersonId = personId,
                Rating = rating,
                Text = text
            });
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> Read(int id)
        {
            // stream the Epub?
            var book = await _bookContentRepo.Find(id);
            return Content(HttpStatusCode.OK, book.Epub);
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> Statistics(int id)
        {
            var statistics = await _bookHistoryRepo.GetAll();
            foreach (var item in statistics.OrderBy(b => b.Created))
            {

            }
            return Ok();
        }
        [HttpGet]
        //[Authorize]
        public async Task<HttpResponseMessage> Cover(int id, int? width = null, int? height = null)
        {


            var book = await _bookRepo.FindWithContent(id);
            using (var ms = new MemoryStream())
            using (var img = Image.FromStream(new MemoryStream(book.Content.Cover)))
            {

                var thumbnail = img.GetThumbnailImage(width ?? img.Width, height ?? img.Height, null, new IntPtr());
                thumbnail.Save(ms, ImageFormat.Png);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(ms.ToArray())
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("image/png")
                        }
                    }
                };
            }
        }

        public async Task<IHttpActionResult> CatchExceptions(Func<Task<Book>> func)
        {
            Book item = null;
            try
            {
                item = await func();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            return Ok(item);
        }

    }
}
