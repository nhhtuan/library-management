using LibraryManagement.DTOs;
using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.Borrow;
using LibraryManagement.DTOs.User;

namespace LibraryManagement.Services.Interfaces;

public interface IBorrowBookService
{
    Task<BorrowTransactionResponse> CreateBorrowTransactionAsync(CreateBorrowTransactionRequest request);
    Task<BorrowTransactionResponse> GetBorrowTransactionByIdAsync(int id);
    Task<PaginatedResponse<BorrowTransactionResponse>> GetBorrowTransactionsByPhoneNumberAsync(string phoneNumber, BorrowTransactionFilterRequest request);
    Task<PaginatedResponse<BorrowTransactionResponse>> GetAllBorrowTransactionsAsync(BorrowTransactionFilterRequest request);
    Task<BorrowTransactionResponse> PatchBorrowTransactionAsync(int id, PatchBorrowTransactionRequest request);
    Task<bool> DeleteBorrowTransactionAsync(int id);
    Task<bool> ReturnBookAsync(int id);
}