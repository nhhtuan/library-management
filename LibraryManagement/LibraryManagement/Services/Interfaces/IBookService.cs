using LibraryManagement.DTOs.Book;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces;

public interface IBookService
{
    Task<PaginatedResponse<BookResponse>> GetAllBooksAsync(BookFilterRequest filter);
    Task<BookResponse> GetBookByIdAsync(int id);
    Task<BookResponse> CreateBookAsync(CreateBookRequest request);
    Task<BookResponse> UpdateBookAsync(int id, UpdateBookRequest request);
    Task<bool> DeleteBookAsync(int id);
}