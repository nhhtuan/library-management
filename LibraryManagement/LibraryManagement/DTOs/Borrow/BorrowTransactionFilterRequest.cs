using LibraryManagement.Enums;

namespace LibraryManagement.DTOs;

public class BorrowTransactionFilterRequest : PaginatedRequest
{
    public bool? IsReturned { get; set; }
    // Sort by
    public string? SortBy { get; set; } = "CreatedAt";
    public SortOrder SortOrder { get; set; }

}