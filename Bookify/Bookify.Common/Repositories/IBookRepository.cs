﻿using System.Threading.Tasks;
using Bookify.Common.Commands.Auth;
using Bookify.Common.Filter;
using Bookify.Common.Models;

namespace Bookify.Common.Repositories
{
    public interface IBookRepository
    {
        Task<DetailedBookDto> GetById(int id);

        Task<IPaginatedEnumerable<BookDto>> GetByFilter(BookFilter filter, int? personId = null);

        Task<BookStatisticsDto> FindForStatistics(int id);

        Task<DetailedBookDto> CreateBook(CreateBookCommand command);

        Task<DetailedBookDto> EditBook(int id, EditBookCommand command);
    }
}
