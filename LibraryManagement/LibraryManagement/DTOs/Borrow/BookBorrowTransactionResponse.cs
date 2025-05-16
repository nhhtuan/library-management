namespace LibraryManagement.DTOs.Borrow;

public class BookBorrowTransactionResponse
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public DateTime PublishedYear { get; set; }
    public string Description { get; set; } = string.Empty;
    public int BorrowedQuantity { get; set; }
}