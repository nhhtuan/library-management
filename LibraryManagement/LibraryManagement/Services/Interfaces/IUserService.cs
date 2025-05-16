using LibraryManagement.DTOs;
using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.User;
using LibraryManagement.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace LibraryManagement.Services.Interfaces;

public interface IUserService
{
    Task<PaginatedResponse<UserDto>> GetAllUsersAsync(PaginatedRequest request);
    Task<UserResponse> GetUserByIdAsync(int id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<UserResponse> PatchUserAsync(int id, PatchUserRequest request);
    Task<bool> DeleteUserAsync(int id);
}
