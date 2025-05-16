using LibraryManagement.DTOs.Borrow;
using LibraryManagement.Enums;
using System.Text.Json.Serialization;

namespace LibraryManagement.DTOs.User
{
    public class PatchBorrowTransactionRequest
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BorrowerName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PhoneNumber { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? DueDate { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BookBorrowTransactionRequest>? BooksBorrow { get; set; }

    }
}