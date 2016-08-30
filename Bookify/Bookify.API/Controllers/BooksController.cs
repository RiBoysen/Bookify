﻿using Bookify.Common.Commands.Auth;
using Bookify.Common.Enums;
using Bookify.Common.Filter;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using Bookify.API.Attributes;
using Bookify.Common.Exceptions;
using Bookify.Common.Models;
using Bookify.Common.Repositories;
using Bookify.DataAccess.Models;

namespace Bookify.API.Controllers
{
    [RoutePrefix("books")]
    public class BooksController : BaseApiController
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookHistoryRepository _bookHistoryRepository;
        private readonly IBookFeedbackRepository _bookFeedbackRepository;
        private readonly IAuthenticationRepository _authRepo;
        private readonly IPersonRepository _personRepository;
        private readonly IBookOrderRepository _bookOrderRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController"/> class.
        /// </summary>
        /// <param name="bookRepository">The book repository. This will be injected.</param>
        /// <param name="bookHistoryRepository">The book history repository. This will be injected.</param>
        /// <param name="bookFeedbackRepository">The book feedback repository. This will be injected.</param>
        /// <param name="authRepo">The authentication repo. This will be injected.</param>
        /// <param name="personRepository">The person repository. This will be injected.</param>
        /// <param name="bookOrderRepository">The book order repository. This will be injected.</param>
        public BooksController(IBookRepository bookRepository, IBookHistoryRepository bookHistoryRepository, IBookFeedbackRepository bookFeedbackRepository, IAuthenticationRepository authRepo, IPersonRepository personRepository, IBookOrderRepository bookOrderRepository)
        {
            this._bookRepository = bookRepository;
            this._bookHistoryRepository = bookHistoryRepository;
            this._bookFeedbackRepository = bookFeedbackRepository;
            this._authRepo = authRepo;
            this._personRepository = personRepository;
            this._bookOrderRepository = bookOrderRepository;
        }

        /// <summary>
        /// Gets books from the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get([FromUri]BookFilter filter = null)
        {
            filter = filter ?? new BookFilter();
            return await this.Try(() => this._bookRepository.GetByFilter(filter));
        }

        /// <summary>
        /// Gets the bowword books from the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        [HttpGet]
        [Auth]
        [Route("mybooks")]
        public async Task<IHttpActionResult> MyBooks([FromUri]BookFilter filter = null)
        {
            filter = filter ?? new BookFilter();
            var person = await this.GetAuthorizedMember(this._authRepo);
            return await this.Try(() => this._bookRepository.GetByFilter(filter, person.PersonDto.Id));
        }

        /// <summary>
        /// Gets the book specified by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            return await this.Try(() => this._bookRepository.GetById(id));
        }

        /// <summary>
        /// Creates a book specified by the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        [Auth]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody]CreateBookCommand command)
        {
            return await this.TryCreate(() => this._bookRepository.CreateBook(command));
        }

        /// <summary>
        /// Updates the book specified by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPatch]
        [Auth]
        [Route("{id}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody]EditBookCommand command)
        {
            return await this.Try(() => this._bookRepository.EditBook(id, command));
        }

        /// <summary>
        /// Deletes the book specified by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete]
        [Auth]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var command = new CreateHistoryCommand
            {
                BookId = id,
                Type = BookHistoryType.Deleted,
                Created = DateTime.Now
            };
            return await this.Try(() => this._bookHistoryRepository.AddHistory(command));
        }

        /// <summary>
        /// Gets the Histories specified by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Auth]
        [Route("{id}/history")]
        public async Task<IHttpActionResult> History(int id)
        {
            return await this.Try(() => this._bookHistoryRepository.GetHistoryForBook(id));
        }

        /// <summary>
        /// Buys the book specified by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/buy")]
        public async Task<IHttpActionResult> Buy(int id, [FromUri]string email)
        {
            return await this.Try(
                async () =>
                    {
                        PersonDto dto;
                        try
                        {
                            var personAuthDto = await this.GetAuthorizedMember(this._authRepo);
                            dto = personAuthDto.PersonDto;
                            if (email != dto.Email) throw new BadRequestException("The email was not identical with the email of the person logged in");
                        }
                        catch (InvalidAccessTokenException)
                        {
                            dto = await this._personRepository.CreatePersonIfNotExists(email);
                        }

                        var command = new CreateOrderCommand
                        {
                            BookId = id,
                            Status = BookOrderStatus.Sold,
                            PersonId = dto.Id
                        };
                        await this._bookOrderRepository.CreateOrder(command);
                    });
        }

        /// <summary>
        /// Borrows the book specified by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        [Auth]
        [Route("{id}/borrow")]
        public async Task<IHttpActionResult> Borrow(int id)
        {
            return await this.Try(
                async () =>
                {
                    var personAuthDto = await this.GetAuthorizedMember(this._authRepo);
                    var dto = personAuthDto.PersonDto;

                    var command = new CreateOrderCommand
                    {
                        BookId = id,
                        Status = BookOrderStatus.Borrowed,
                        PersonId = dto.Id
                    };
                    await this._bookOrderRepository.CreateOrder(command);
                });
        }

        /// <summary>
        /// Gets the Statisticses specified by the book identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Auth]
        [Route("{id}/statistics")]
        public async Task<IHttpActionResult> Statistics(int id)
        {
            return await this.Try(async () => await this._bookRepository.FindForStatistics(id));
        }
    }
}
