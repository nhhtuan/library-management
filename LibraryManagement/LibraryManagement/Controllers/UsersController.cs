using AutoMapper;
using LibraryManagement.Data;
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
        try
        {
            var userResponse = await _userService.GetUserByIdAsync(id);
            return Ok(userResponse);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
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
        try
        {
            var userResponse = await _userService.PatchUserAsync(id, request);
            return Ok(userResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return BadRequest("User deletion failed");

            return Ok("User deleted successfully");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

}