using System.ComponentModel.DataAnnotations;
using LibraryManagement.Enums;

namespace LibraryManagement.DTOs.Book
{
    public class BookFilterRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Genre { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }

        public string? SortBy { get; set; } = "CreatedAt";
        // SortBy can be "Title", "Author", "Genre", "PublishedYear", "CreatedAt"
        public SortOrder SortOrder { get; set; }

    }
}