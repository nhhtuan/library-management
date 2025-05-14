namespace LibraryManagement.DTOs.Book
{
    public class BookResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime PublishedYear { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}