using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.DTOs.Book;
using LibraryManagement.DTOs.User;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services;

public class UserService : IUserService
{
    private readonly LibraryDbContext _db;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _hasher;
    public UserService(LibraryDbContext db, IMapper mapper, IPasswordHasher<User> hasher)
    {
        _mapper = mapper;
        _db = db;
        _hasher = hasher;
    }

    public async Task<PaginatedResponse<UserDto>> GetAllUsersAsync(PaginatedRequest request)
    {
        // var users = await _db.Users
        // .Where(u => u.DeletedAt == null)
        //     .ToListAsync();

        IQueryable<User> query = _db.Users.AsNoTracking();
        var totalCount = await query.Where(q => q.DeletedAt == null).CountAsync();

        // Apply pagination
        if (request.Page < 1) throw new ArgumentOutOfRangeException("Page must be greater than 0");
        if (request.PageSize < 1 || request.PageSize > 50) throw new ArgumentOutOfRangeException("PageSize must be greater than 0 and less than or equal to 50");
        var users = await query
            .Where(u => u.DeletedAt == null)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var userDtoResponse = _mapper.Map<List<UserDto>>(users);
        return new PaginatedResponse<UserDto>
        {
            Items = userDtoResponse,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<UserResponse> GetUserByIdAsync(int id)
    {
        var user = await _db.Users.Where(u => u.Id == id && u.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (user == null)
            throw new KeyNotFoundException("User not found");

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {

        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            Username = request.Username,
            Role = request.Role
        };

        //var hasher = new PasswordHasher<User>();
        //user.PasswordHash = hasher.HashPassword(user, request.Password);
        user.PasswordHash = _hasher.HashPassword(user, request.Password);
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var user = await _db.Users.Where(u => u.Id == id && u.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.Role = request.UserRole;
        user.FullName = request.FullName;
        user.DateOfBirth = request.DateOfBirth;

        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _db.Users.Where(u => u.Id == id && u.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (user == null)
            throw new KeyNotFoundException("User not found or already deleted");

        // Soft delete
        user.DeletedAt = DateTime.UtcNow;
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<UserResponse> PatchUserAsync(int id, PatchUserRequest request)
    {
        var user = await _db.Users.Where(u => u.Id == id && u.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (request.UserRole.HasValue)
            user.Role = request.UserRole.Value;

        if (request.FullName != null)
            user.FullName = request.FullName;

        if (request.DateOfBirth.HasValue)
            user.DateOfBirth = request.DateOfBirth;

        user.UpdatedAt = DateTime.UtcNow;

        _db.Users.Update(user);
        await _db.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }

}