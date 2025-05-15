using LibraryManagement.DTOs.Borrow;

namespace LibraryManagement.Services.Interfaces;

public interface IBorrowBookService
{
    Task<BorrowTransactionResponse> CreateBorrowTransactionAsync(CreateBorrowTransactionRequest request);
    Task<BorrowTransactionResponse> GetBorrowTransactionByIdAsync(int id);
    Task<BorrowTransactionResponse> GetBorrowTransactionByPhoneNumberAsync(string phoneNumber);
    Task<IEnumerable<BorrowTransactionResponse>> GetAllBorrowTransactionsAsync();
    Task<bool> DeleteBorrowTransactionAsync(int id);
}