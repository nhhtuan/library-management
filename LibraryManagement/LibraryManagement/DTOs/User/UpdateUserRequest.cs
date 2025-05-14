
using LibraryManagement.Enums;

namespace LibraryManagement.DTOs.User
{
    public class UpdateUserRequest
    {
        public UserRole UserRole { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
    }
}