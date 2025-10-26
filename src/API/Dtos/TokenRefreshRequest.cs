namespace SGS.TaskTracker.Dtos
{
    public class TokenRefreshRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
