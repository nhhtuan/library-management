
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DB and JWT Service
builder.Services.AddDbContext<LibraryDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<JwtService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Seed admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    if (!db.Users.Any())
    {
        var hasher = new PasswordHasher<User>();

        var admin = new User
        {
            Username = "admin",
            Role = UserRole.Admin
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");

        var librarian = new User
        {
            Username = "librarian",
            Role = UserRole.Librarian
        };
        librarian.PasswordHash = hasher.HashPassword(librarian, "lib123");

        db.Users.AddRange(admin, librarian);
        db.SaveChanges();
    }
}

app.Run();
