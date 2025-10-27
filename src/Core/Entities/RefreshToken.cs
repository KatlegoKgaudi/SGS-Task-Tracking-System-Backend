namespace SGS.TaskTracker.Core.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
