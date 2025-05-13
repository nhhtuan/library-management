using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.User
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.Librarian;
    }
}