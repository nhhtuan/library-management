using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.Borrow
{
    public class BookBorrowTransactionRequest
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Quantity must be greater than 0 and less than or equal to 5.")]
        public int Quantity { get; set; }
    }
}