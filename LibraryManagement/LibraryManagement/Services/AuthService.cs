using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs.Auth;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services;

public class AuthService : IAuthService
{
    private readonly LibraryDbContext _db;
    private readonly JwtService _jwt;

    public AuthService(LibraryDbContext db, JwtService jwt, IMapper mapper)
    {
        _jwt = jwt;
        _db = db;
    }


    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid username or password");

        var accessToken = _jwt.GenerateAccessToken(user.Username, user.Role.ToString());
        var refreshToken = _jwt.GenerateRefreshToken();
        var refreshTokenExpiryTime = _jwt.GenerateRefreshTokenExpiryTime();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

        await _db.SaveChangesAsync();
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = refreshTokenExpiryTime,
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // Generate new access token and refresh token
        var newAccessToken = _jwt.GenerateAccessToken(user.Username, user.Role.ToString());
        var newRefreshToken = _jwt.GenerateRefreshToken();
        var newRefreshTokenExpiryTime = _jwt.GenerateRefreshTokenExpiryTime();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = newRefreshTokenExpiryTime;

        await _db.SaveChangesAsync();
        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = newRefreshTokenExpiryTime,
        };
    }

    public async Task LogoutAsync(string username)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Remove the refresh token and expiry time
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _db.SaveChangesAsync();
    }
}