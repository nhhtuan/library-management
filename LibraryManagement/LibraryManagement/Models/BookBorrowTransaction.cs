using LibraryManagement.Models;

public class BookBorrowTransaction
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int BorrowTransactionId { get; set; }
    public BorrowTransaction BorrowTransaction { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
