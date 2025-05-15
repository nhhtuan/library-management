using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Borrow;
using LibraryManagement.Models;

namespace LibraryManagement.Services;

public class BorrowBookService
{
    private readonly IMapper _mapper;
    private readonly LibraryDbContext _db;

    public BorrowBookService(LibraryDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }


    public async Task<BorrowTransactionResponse> CreateBorrowTransactionAsync(CreateBorrowTransactionRequest request)
    {
        var transaction = _mapper.Map<BorrowTransaction>(request);
        await _db.BorrowTransactions.AddAsync(transaction);
        await _db.SaveChangesAsync();
        return _mapper.Map<BorrowTransactionResponse>(transaction);
    }

}