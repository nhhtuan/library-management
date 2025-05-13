using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs.User;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services;

public class UserService : IUserService
{
    private readonly LibraryDbContext _db;
    private readonly IMapper _mapper;

    public UserService(LibraryDbContext db, IMapper mapper)
    {
        _mapper = mapper;
        _db = db;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _db.Users
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserResponse> GetUserByIdAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Role = request.Role
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }
}