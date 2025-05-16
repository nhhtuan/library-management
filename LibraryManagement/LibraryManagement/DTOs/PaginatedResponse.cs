namespace LibraryManagement.DTOs.Book
{
    public class PaginatedResponse<T>
    {
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public List<T> Items { get; set; } = new List<T>();
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}