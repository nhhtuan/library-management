namespace LibraryManagement.Models;

public class BorrowTransaction
{
    public int Id { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public List<Book> BorrowedBooks { get; set; } = new List<Book>();

    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !ReturnDate.HasValue && DateTime.UtcNow > DueDate;
}
