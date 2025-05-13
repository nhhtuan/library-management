using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(LibraryDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers(){
        var users = await db.Users
            .Select(u => new { u.Id, u.Username, Role = u.Role.ToString() })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id){
        var user = await db.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found");
        return Ok(user);
    }

    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] User user){
        if (user == null)
            return BadRequest("User data is required");
        
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, user.PasswordHash);
        
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

}