using SGS.TaskTracker.Core.Entities;
using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Dtos
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
