using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.DTOs.User;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{

    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    private readonly LibraryDbContext _context;

    public UserController(IUserService userService, IMapper mapper, LibraryDbContext context)
    {
        _userService = userService;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginatedRequest request)
    {
        var users = await _userService.GetAllUsersAsync(request);
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        var userResponse = await _userService.GetUserByIdAsync(id);
        return Ok(userResponse);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var createdUser = await _userService.CreateUserAsync(request);
        if (createdUser == null)
            return BadRequest("User creation failed");
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }


    // [HttpPatch("{id}")]
    // [Authorize(Roles = "Admin")]
    // public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    // {
    //     try
    //     {
    //         var userResponse = await _userService.UpdateUserAsync(id, request);
    //         return Ok(userResponse);
    //     }
    //     catch (Exception ex)
    //     {
    //         return NotFound(ex.Message);
    //     }
    // }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchUser(int id, [FromBody] PatchUserRequest request)
    {
        var userResponse = await _userService.PatchUserAsync(id, request);
        return Ok(userResponse);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id) // Soft delete
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return BadRequest("User deletion failed");

        return Ok("User deleted successfully");

    }

}