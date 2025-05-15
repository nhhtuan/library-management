namespace LibraryManagement.DTOs.Borrow;

public class BorrowTransactionResponse
{
    public int Id { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public List<BorrowedBookResponse> BorrowedBooks { get; set; } = new List<BorrowedBookResponse>();

    public bool IsReturned { get; set; }
    public bool IsOverdue { get; set; }
}