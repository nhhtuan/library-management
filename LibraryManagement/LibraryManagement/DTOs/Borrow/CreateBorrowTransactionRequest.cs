using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.Borrow
{
    public class CreateBorrowTransactionRequest
    {
        public string BorrowerName { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        [MinLength(1, ErrorMessage = "At least one book must be borrowed.")]
        public List<BookBorrowTransactionRequest> BooksBorrow { get; set; } = new List<BookBorrowTransactionRequest>();
        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime DueDate { get; set; }
    }
}