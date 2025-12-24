using System;

namespace ApiTests.Models
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Isbn { get; set; }
        public DateTime PublishedDate { get; set; }

        public BookDto Clone()
        {
            return (BookDto)this.MemberwiseClone();
        }
    }

    public class BookCreateDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Isbn { get; set; }
        public DateTime PublishedDate { get; set; }

        public BookCreateDto Clone()
        {
            return (BookCreateDto)this.MemberwiseClone();
        }
    }
}