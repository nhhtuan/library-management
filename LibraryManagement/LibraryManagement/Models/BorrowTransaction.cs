namespace LibraryManagement.Models;

public class BorrowTransaction
{
    public int Id { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public List<BookBorrowTransaction> BookBorrowTransactions { get; set; } = new();

    public int TotalBooksBorrowed => BookBorrowTransactions.Sum(b => b.Quantity);

    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !ReturnDate.HasValue && DateTime.UtcNow > DueDate;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
