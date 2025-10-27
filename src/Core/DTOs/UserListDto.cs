using SGS.TaskTracker.Core.Entities;

namespace SGS.TaskTracker.Dtos
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Userame { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int TaskCount { get; set; }
    }
}
