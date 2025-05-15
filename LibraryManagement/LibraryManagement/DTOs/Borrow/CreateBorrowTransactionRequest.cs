namespace LibraryManagement.DTOs.Borrow
{
    public class CreateBorrowTransactionRequest
    {
        public string BorrowerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<int> BookIds { get; set; } = new List<int>();

        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
    }
}