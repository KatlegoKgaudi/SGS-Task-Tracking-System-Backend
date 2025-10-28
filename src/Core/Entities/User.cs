namespace SGS.TaskTracker.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role {  get; set; } = UserRole.Regular;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

public enum UserRole
{
    Regular = 0,
    Admin = 1
}
