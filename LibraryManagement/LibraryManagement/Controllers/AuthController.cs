
using LibraryManagement.Services;
using LibraryManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Models;

namespace LibraryAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(LibraryDbContext db, JwtService jwt) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await db.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        //var hasher = new PasswordHasher<User>();

        //if (user == null || user.PasswordHash != hasher.HashPassword(user, request.Password))
        //    return Unauthorized("Invalid username or password");

        //if (user == null)
        //{
        //    return Unauthorized("User not found");
        //}

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user!, user!.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed || user == null)
            return Unauthorized("Invalid username or password");

        var token = jwt.GenerateToken(user.Username, user.Role.ToString());
        return Ok(new { token });
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
