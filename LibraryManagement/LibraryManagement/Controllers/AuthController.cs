
using LibraryManagement.Services;
using LibraryManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly LibraryDbContext _db;
    private readonly JwtService _jwt;


    public AuthController(LibraryDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user!, user!.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed || user == null)
            return Unauthorized("Invalid username or password");

        var accessToken = _jwt.GenerateAccessToken(user.Username, user.Role.ToString());

        var refreshToken = _jwt.GenerateRefreshToken();
        var refreshTokenExpiryTime = _jwt.GenerateRefreshTokenExpiryTime();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

        await _db.SaveChangesAsync();
        return Ok(new
        {
            accessToken,
            refreshToken,
            refreshTokenExpiryTime,
            username = user.Username,
            role = user.Role.ToString()
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {

        var user = await _db.Users.SingleOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return BadRequest("Invalid or expired refresh token");

        // Generate new access token and refresh token
        var newAccessToken = _jwt.GenerateAccessToken(user.Username, user.Role.ToString());
        var newRefreshToken = _jwt.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        var newRefreshTokenExpiryTime = _jwt.GenerateRefreshTokenExpiryTime();

        await _db.SaveChangesAsync();
        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken,
            refreshTokenExpiryTime = newRefreshTokenExpiryTime
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null)
            return NotFound("User not found");

        // Remove the refresh token and expiry time
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _db.SaveChangesAsync();
        return Ok("Logged out successfully");
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = "";
    }
}
