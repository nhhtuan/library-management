using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.User;
using LibraryManagement.Enums;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LibraryManagement.Services;


public class BookService : IBookService
{
    private readonly IMapper _mapper;
    private readonly LibraryDbContext _db;

    public BookService(LibraryDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    public async Task<PaginatedResponse<BookResponse>> GetAllBooksAsync(BookFilterRequest filter)
    {
        IQueryable<Book> query = _db.Books.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(b => b.Title.Contains(filter.Search) || b.Author.Contains(filter.Search));
        }

        if (!string.IsNullOrWhiteSpace(filter.Genre))
        {
            query = query.Where(b => b.Genre.Contains(filter.Genre));
        }

        if (filter.YearFrom.HasValue)
        {
            query = query.Where(b => b.PublishedYear.Year >= filter.YearFrom.Value);
        }

        if (filter.YearTo.HasValue)
        {
            query = query.Where(b => b.PublishedYear.Year <= filter.YearTo.Value);
        }

        // Count total records
        var totalCount = await query.Where(q => q.DeletedAt == null).CountAsync();

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            var sortOrder = filter.SortOrder == SortOrder.Desc ? "descending" : "ascending";
            query = query.OrderBy($"{filter.SortBy} {sortOrder}");
        }
        else
        {
            query = filter.SortOrder == SortOrder.Desc ?
                    query.OrderByDescending(b => b.Title) :
                    query.OrderBy(b => b.Title);

        }

        // Apply pagination
        if (filter.Page < 1) throw new ArgumentOutOfRangeException("Page must be greater than 0");
        if (filter.PageSize < 1 || filter.PageSize > 50) throw new ArgumentOutOfRangeException("PageSize must be greater than 0 and less than or equal to 50");
        var books = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Where(b => b.DeletedAt == null)
            .ToListAsync();

        var bookResponses = _mapper.Map<List<BookResponse>>(books);

        return new PaginatedResponse<BookResponse>
        {
            Items = bookResponses,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
    public async Task<BookResponse> GetBookByIdAsync(int id)
    {
        var book = await _db.Books
            .Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        return _mapper.Map<BookResponse>(book);
    }

    public async Task<BookResponse> CreateBookAsync(CreateBookRequest request)
    {

        // Check if the book already exists
        var existingBook = await _db.Books
            .Where(b => b.Title == request.Title && b.Author == request.Author && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (existingBook != null)
            throw new InvalidOperationException("Book already exists");
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Genre = request.Genre,
            PublishedYear = request.PublishedYear,
            Description = request.Description,
            Quantity = request.Quantity
        };

        await _db.Books.AddAsync(book);
        await _db.SaveChangesAsync();

        return _mapper.Map<BookResponse>(book);
    }
    public async Task<BookResponse> UpdateBookAsync(int id, UpdateBookRequest request)
    {
        var book = await _db.Books.Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        book.Title = request.Title;
        book.Author = request.Author;
        book.Genre = request.Genre;
        book.PublishedYear = request.PublishedYear;
        book.Description = request.Description;
        book.Quantity = request.Quantity;

        _db.Books.Update(book);
        await _db.SaveChangesAsync();

        return _mapper.Map<BookResponse>(book);
    }

    public async Task<BookResponse> PatchBookAsync(int id, PatchBookRequest request)
    {
        var book = await _db.Books.Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        if (request.Title != null)
            book.Title = request.Title;
        if (request.Author != null)
            book.Author = request.Author;
        if (request.Genre != null)
            book.Genre = request.Genre;
        if (request.PublishedYear.HasValue)
            book.PublishedYear = request.PublishedYear.Value;
        if (request.Description != null)
            book.Description = request.Description;
        if (request.Quantity.HasValue)
            book.Quantity = request.Quantity.Value;

        book.UpdatedAt = DateTime.UtcNow;
        _db.Books.Update(book);
        await _db.SaveChangesAsync();

        return _mapper.Map<BookResponse>(book);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _db.Books.Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (book == null)
            throw new KeyNotFoundException("Book not found");

        // Soft delete
        book.DeletedAt = DateTime.UtcNow;
        _db.Books.Update(book);
        await _db.SaveChangesAsync();

        return true;
    }

}