using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.User;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces;

public interface IBookService
{
    Task<PaginatedResponse<BookResponse>> GetAllBooksAsync(BookFilterRequest filter);
    Task<BookResponse> GetBookByIdAsync(int id);
    Task<BookResponse> CreateBookAsync(CreateBookRequest request);
    Task<BookResponse> UpdateBookAsync(int id, UpdateBookRequest request);
    Task<BookResponse> PatchBookAsync(int id, PatchBookRequest request);
    Task<bool> DeleteBookAsync(int id);
}