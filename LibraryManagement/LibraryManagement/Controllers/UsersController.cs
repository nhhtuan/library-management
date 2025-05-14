using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.DTOs.User;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{

    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound("User not found");
        return Ok(user);
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

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound("User not found");

        _mapper.Map(request, user);
        var userResponse = await _userService.UpdateUserAsync(id, request);

        return Ok(userResponse);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound("User not found");

        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return BadRequest("User deletion failed");

        return Ok("User deleted successfully");
    }

}