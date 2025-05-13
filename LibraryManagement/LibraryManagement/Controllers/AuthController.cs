
using LibraryManagement.Services;
using LibraryManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.DTOs.Auth;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;


    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var loginResponse = await _authService.LoginAsync(request);

            return Ok(loginResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {

        try
        {
            var refreshTokenResponse = await _authService.RefreshTokenAsync(request);

            return Ok(refreshTokenResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("User not authenticated");

            await _authService.LogoutAsync(username);
            return Ok("Logged out successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
