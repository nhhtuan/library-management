using System.Linq.Dynamic.Core;
using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.User;
using LibraryManagement.Enums;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services;

public class BorrowBookService : IBorrowBookService
{
    private readonly IMapper _mapper;
    private readonly LibraryDbContext _db;

    public BorrowBookService(LibraryDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }


    public async Task<PaginatedResponse<BorrowTransactionResponse>> GetAllBorrowTransactionsAsync(BorrowTransactionFilterRequest request)
    {

        IQueryable<BorrowTransaction> query = _db.BorrowTransactions.AsNoTracking();


        // Apply filters
        if (request.IsReturned.HasValue)
        {
            if (request.IsReturned.Value)
            {
                query = query.Where(b => b.ReturnDate != null);
            }
            else
            {
                query = query.Where(b => b.ReturnDate == null);
            }
        }


        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var sortOrder = request.SortOrder == SortOrder.Desc ? "descending" : "ascending";
            query = query.OrderBy($"{request.SortBy} {sortOrder}");
        }
        else
        {
            query = request.SortOrder == SortOrder.Desc ?
                    query.OrderByDescending(b => b.CreatedAt) :
                    query.OrderBy(b => b.CreatedAt);

        }

        var totalCount = await query.Where(q => q.DeletedAt == null).CountAsync();

        // Apply pagination
        if (request.Page < 1) throw new ArgumentOutOfRangeException("Page must be greater than 0");
        if (request.PageSize < 1 || request.PageSize > 50) throw new ArgumentOutOfRangeException("PageSize must be greater than 0 and less than or equal to 50");
        var borrowTransactions = await query
            .Include(b => b.BookBorrowTransactions)
            .ThenInclude(bb => bb.Book)
            .Where(u => u.DeletedAt == null)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var borrowTransactionResponse = _mapper.Map<List<BorrowTransactionResponse>>(borrowTransactions);

        return new PaginatedResponse<BorrowTransactionResponse>
        {
            Items = borrowTransactionResponse,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }


    public async Task<BorrowTransactionResponse> CreateBorrowTransactionAsync(CreateBorrowTransactionRequest request)
    {

        // Validate the request
        validateDateTime(request.BorrowDate, request.DueDate);

        // Check if the user has already borrowed 5 books
        var result = await IsHoldMax5BooksAsync(request.PhoneNumber, request.BooksBorrow.Sum(b => b.Quantity));

        if (!result)
            throw new InvalidOperationException("You cannot hold more than 5 books");

        var borrowTransaction = _mapper.Map<BorrowTransaction>(request);

        foreach (var bookBorrow in request.BooksBorrow)
        {
            var book = await _db.Books.Where(b => b.Id == bookBorrow.BookId && b.DeletedAt == null).FirstOrDefaultAsync();
            if (book == null || book.Quantity < 1)
                throw new KeyNotFoundException($"Book with ID {bookBorrow.BookId} not found");

            book.Quantity -= bookBorrow.Quantity;

            borrowTransaction.BookBorrowTransactions.Add(new BookBorrowTransaction
            {
                BookId = book.Id,
                BorrowTransaction = borrowTransaction,
                Quantity = bookBorrow.Quantity
            });
        }

        await _db.BorrowTransactions.AddAsync(borrowTransaction);
        await _db.SaveChangesAsync();
        return _mapper.Map<BorrowTransactionResponse>(borrowTransaction);
    }

    public void validateDateTime(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date cannot be greater than end date");

        if (startDate < DateTime.UtcNow)
            throw new ArgumentException("Start date cannot be in the past");
    }

    public async Task<bool> IsHoldMax5BooksAsync(string phoneNumber, int bookRequestNumber)
    {
        var totalBooksBorrowed = await _db.BorrowTransactions
    .Where(bt => bt.PhoneNumber == phoneNumber && bt.DeletedAt == null && bt.ReturnDate == null)
    .SelectMany(bt => bt.BookBorrowTransactions)
    .SumAsync(bbt => bbt.Quantity);

        if (totalBooksBorrowed + bookRequestNumber > 5)
            return false;

        return true;
    }


    public async Task<BorrowTransactionResponse> GetBorrowTransactionByIdAsync(int id)
    {
        var borrowTransaction = await _db.BorrowTransactions
            .Include(b => b.BookBorrowTransactions)
            .ThenInclude(bb => bb.Book)
            .Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (borrowTransaction == null)
            throw new KeyNotFoundException("Borrow transaction not found");

        return _mapper.Map<BorrowTransactionResponse>(borrowTransaction);
    }


    public async Task<PaginatedResponse<BorrowTransactionResponse>> GetBorrowTransactionsByPhoneNumberAsync(string phoneNumber, BorrowTransactionFilterRequest request)
    {

        IQueryable<BorrowTransaction> query = _db.BorrowTransactions.AsNoTracking();

        // Apply filters
        if (request.IsReturned.HasValue)
        {
            if (request.IsReturned.Value)
            {
                query = query.Where(b => b.ReturnDate != null);
            }
            else
            {
                query = query.Where(b => b.ReturnDate == null);
            }
        }


        // Apply sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var sortOrder = request.SortOrder == SortOrder.Desc ? "descending" : "ascending";
            query = query.OrderBy($"{request.SortBy} {sortOrder}");
        }
        else
        {
            query = request.SortOrder == SortOrder.Desc ?
                    query.OrderByDescending(b => b.CreatedAt) :
                    query.OrderBy(b => b.CreatedAt);

        }

        var totalCount = await query.Where(q => q.DeletedAt == null).CountAsync();

        // Apply pagination
        if (request.Page < 1) throw new ArgumentOutOfRangeException("Page must be greater than 0");
        if (request.PageSize < 1 || request.PageSize > 50) throw new ArgumentOutOfRangeException("PageSize must be greater than 0 and less than or equal to 50");
        var borrowTransactions = await query
            .Include(b => b.BookBorrowTransactions)
            .ThenInclude(bb => bb.Book)
            .Where(b => b.PhoneNumber == phoneNumber && b.DeletedAt == null)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        if (borrowTransactions == null)
            throw new KeyNotFoundException("Borrow transaction not found");

        var borrowTransactionResponse = _mapper.Map<List<BorrowTransactionResponse>>(borrowTransactions);

        return new PaginatedResponse<BorrowTransactionResponse>
        {
            Items = borrowTransactionResponse,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }


    public async Task<BorrowTransactionResponse> PatchBorrowTransactionAsync(int id, PatchBorrowTransactionRequest request)
    {
        // Sử dụng transaction database để đảm bảo atomic operation
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var borrowTransaction = await _db.BorrowTransactions
                .Include(b => b.BookBorrowTransactions)
                .ThenInclude(bb => bb.Book)
                .Where(b => b.Id == id && b.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (borrowTransaction == null)
                throw new KeyNotFoundException("Borrow transaction not found");

            // Ngăn chặn việc update cho giao dịch đã trả sách
            if (borrowTransaction.ReturnDate != null)
                throw new InvalidOperationException("Cannot modify a transaction that has been returned");

            if (request.BorrowerName != null)
                borrowTransaction.BorrowerName = request.BorrowerName;
            if (request.PhoneNumber != null)
                borrowTransaction.PhoneNumber = request.PhoneNumber;
            if (request.DueDate.HasValue)
                borrowTransaction.DueDate = request.DueDate.Value;

            // Xử lý trường hợp trả sách
            if (request.BooksBorrow != null)
            {
                // Map BookId -> old BookBorrowTransaction
                var oldBooksMap = borrowTransaction.BookBorrowTransactions
                    .ToDictionary(bb => bb.BookId, bb => bb);

                var newBookIds = request.BooksBorrow.Select(b => b.BookId).ToHashSet();

                // Xử lý từng sách trong request
                foreach (var bookBorrow in request.BooksBorrow)
                {
                    var book = await _db.Books
                        .FirstOrDefaultAsync(b => b.Id == bookBorrow.BookId && b.DeletedAt == null);
                    if (book == null)
                        throw new KeyNotFoundException($"Book with ID {bookBorrow.BookId} not found");

                    if (oldBooksMap.TryGetValue(bookBorrow.BookId, out var existing))
                    {
                        // Sách đã tồn tại, tính chênh lệch
                        var diff = bookBorrow.Quantity - existing.Quantity;
                        if (book.Quantity < diff)
                            throw new InvalidOperationException("Not enough quantity available.");
                        book.Quantity -= diff;
                        existing.Quantity = bookBorrow.Quantity; // cập nhật
                    }
                    else
                    {
                        // Sách mới, trừ kho và thêm mới
                        if (book.Quantity < bookBorrow.Quantity)
                            throw new InvalidOperationException("Not enough quantity available.");
                        book.Quantity -= bookBorrow.Quantity;
                        borrowTransaction.BookBorrowTransactions.Add(new BookBorrowTransaction
                        {
                            BookId = book.Id,
                            Quantity = bookBorrow.Quantity
                        });
                    }
                }

                // Xóa các sách không còn trong request
                var toRemove = borrowTransaction.BookBorrowTransactions
                    .Where(bb => !newBookIds.Contains(bb.BookId))
                    .ToList();
                foreach (var removeItem in toRemove)
                {
                    var book = await _db.Books.FindAsync(removeItem.BookId);
                    if (book != null)
                        book.Quantity += removeItem.Quantity;
                    borrowTransaction.BookBorrowTransactions.Remove(removeItem);
                }

            }


            borrowTransaction.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Validate max 5 books
            //var result = await IsHoldMax5BooksAsync(borrowTransaction.PhoneNumber, borrowTransaction.TotalBooksBorrowed);

            // Tính lại tổng số sách đang giữ của user này (trừ transaction hiện tại)
            var totalBooksHeld = await _db.BorrowTransactions
                .Where(bt => bt.PhoneNumber == borrowTransaction.PhoneNumber
                    && bt.DeletedAt == null
                    && bt.ReturnDate == null
                    && bt.Id != borrowTransaction.Id) // Trừ chính giao dịch đang update
                .SelectMany(bt => bt.BookBorrowTransactions)
                .SumAsync(bbt => bbt.Quantity);

            if (totalBooksHeld + borrowTransaction.TotalBooksBorrowed > 5)
                throw new InvalidOperationException("You cannot hold more than 5 books");

            // Commit transaction nếu mọi thứ ổn
            await transaction.CommitAsync();

            return _mapper.Map<BorrowTransactionResponse>(borrowTransaction);
        }
        catch (Exception)
        {
            // Rollback transaction nếu có lỗi
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<bool> DeleteBorrowTransactionAsync(int id)
    {
        var borrowTransaction = await _db.BorrowTransactions
            .Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (borrowTransaction == null)
            throw new KeyNotFoundException("Borrow transaction not found");

        borrowTransaction.DeletedAt = DateTime.UtcNow;
        _db.BorrowTransactions.Update(borrowTransaction);
        await _db.SaveChangesAsync();
        return true;
    }


    public async Task<bool> ReturnBookAsync(int id)
    {
        var borrowTransaction = await _db.BorrowTransactions
            .Include(b => b.BookBorrowTransactions)
            .ThenInclude(bb => bb.Book)
            .Where(b => b.Id == id && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (borrowTransaction == null)
            throw new KeyNotFoundException("Borrow transaction not found");

        // Ngăn chặn việc trả sách cho giao dịch đã trả sách
        if (borrowTransaction.ReturnDate != null)
            throw new InvalidOperationException("Cannot return a transaction that has already been returned");

        borrowTransaction.ReturnDate = DateTime.UtcNow;

        foreach (var bookBorrow in borrowTransaction.BookBorrowTransactions)
        {
            var book = await _db.Books.FindAsync(bookBorrow.BookId);
            if (book != null)
                book.Quantity += bookBorrow.Quantity;
        }

        await _db.SaveChangesAsync();
        return true;
    }

}