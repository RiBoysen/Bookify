﻿namespace Bookify.Common.Commands.Auth
{
    public class CreateBookCommand
    {
        public string ISBN;
        public string Title { get; set; }
        public string Summary { get; set; }
        public int AuthorId { get; set; }
        public int PublishYear { get; set; }
        public int[] Genres { get; set; }
        public int PublisherId { get; set; }
        public string Language { get; set; }
        public int? CopiesAvailable { get; set; } = 0;
        public int? PageCount { get; set; } = 0;
        public decimal Price { get; set; }
    }
}