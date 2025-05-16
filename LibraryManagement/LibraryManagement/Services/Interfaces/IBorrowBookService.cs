using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.User;

namespace LibraryManagement.Services.Interfaces;

public interface IBorrowBookService
{
    Task<BorrowTransactionResponse> CreateBorrowTransactionAsync(CreateBorrowTransactionRequest request);
    Task<BorrowTransactionResponse> GetBorrowTransactionByIdAsync(int id);
    Task<IEnumerable<BorrowTransactionResponse>> GetBorrowTransactionsByPhoneNumberAsync(string phoneNumber);
    Task<IEnumerable<BorrowTransactionResponse>> GetAllBorrowTransactionsAsync();
    Task<BorrowTransactionResponse> PatchBorrowTransactionAsync(int id, PatchBorrowTransactionRequest request);
    Task<bool> DeleteBorrowTransactionAsync(int id);
}