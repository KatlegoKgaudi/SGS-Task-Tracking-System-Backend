namespace SGS.TaskTracker.Models
{
    public class AuthResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; } 
        public string Message { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool Success { get; set; }
    }
}
