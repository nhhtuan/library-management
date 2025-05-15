using LibraryManagement.DTOs.User;
using LibraryManagement.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace LibraryManagement.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserResponse> GetUserByIdAsync(int id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<UserResponse> PatchUserAsync(int id, PatchUserRequest request);
    Task<bool> DeleteUserAsync(int id);
}
