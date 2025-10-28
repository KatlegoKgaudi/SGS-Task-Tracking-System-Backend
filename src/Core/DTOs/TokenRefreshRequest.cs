namespace SGS.TaskTracker.Core.DTOs
{
    public class TokenRefreshRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
