using SGS.TaskTracker.Core.Entities;
using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Dtos
{
    public class UserUpdateDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public UserRole? Role { get; set; }
    }
}
