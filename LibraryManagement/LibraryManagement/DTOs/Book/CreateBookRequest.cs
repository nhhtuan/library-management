using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.Book
{
    public class CreateBookRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required]
        public string Genre { get; set; } = string.Empty;
        [Required]
        public DateTime PublishedYear { get; set; }
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}