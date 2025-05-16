using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.User;
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


    public async Task<IEnumerable<BorrowTransactionResponse>> GetAllBorrowTransactionsAsync()
    {
        var borrowTransactions = await _db.BorrowTransactions
            .Include(b => b.BookBorrowTransactions)
            .ThenInclude(bb => bb.Book)
            .Where(b => b.DeletedAt == null)
            .ToListAsync();

        return _mapper.Map<IEnumerable<BorrowTransactionResponse>>(borrowTransactions);
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


    public async Task<IEnumerable<BorrowTransactionResponse>> GetBorrowTransactionsByPhoneNumberAsync(string phoneNumber)
    {
        var borrowTransaction = await _db.BorrowTransactions
            .Include(b => b.BookBorrowTransactions)
            .ThenInclude(bb => bb.Book)
            .Where(b => b.PhoneNumber == phoneNumber && b.DeletedAt == null)
            .ToListAsync();

        if (borrowTransaction == null)
            throw new KeyNotFoundException("Borrow transaction not found");

        return _mapper.Map<IEnumerable<BorrowTransactionResponse>>(borrowTransaction);
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
            if (borrowTransaction.ReturnDate != null && !request.ReturnDate.HasValue)
                throw new InvalidOperationException("Cannot modify a transaction that has been returned");

            // Xử lý các trường thông thường
            if (request.BorrowerName != null)
                borrowTransaction.BorrowerName = request.BorrowerName;
            if (request.PhoneNumber != null)
                borrowTransaction.PhoneNumber = request.PhoneNumber;
            if (request.DueDate.HasValue)
                borrowTransaction.DueDate = request.DueDate.Value;
            if (request.ReturnDate.HasValue)
            {
                borrowTransaction.ReturnDate = request.ReturnDate.Value;
                // Nếu có ngày trả sách, cập nhật lại số lượng sách trong kho
                foreach (var bookBorrow in borrowTransaction.BookBorrowTransactions)
                {
                    var book = await _db.Books.FindAsync(bookBorrow.BookId);
                    if (book != null)
                        book.Quantity += bookBorrow.Quantity;
                }
            }

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

                // Validate max 5 books
                var result = await IsHoldMax5BooksAsync(borrowTransaction.PhoneNumber, borrowTransaction.TotalBooksBorrowed);
                if (!result)
                    throw new InvalidOperationException("You cannot hold more than 5 books");
            }


            borrowTransaction.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

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

}