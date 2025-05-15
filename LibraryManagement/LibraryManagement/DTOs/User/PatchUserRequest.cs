using LibraryManagement.Enums;
using System.Text.Json.Serialization;

namespace LibraryManagement.DTOs.User
{
    public class PatchUserRequest
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserRole? UserRole { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FullName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? DateOfBirth { get; set; }
    }
}