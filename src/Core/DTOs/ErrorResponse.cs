namespace SGS.TaskTracker.Core.DTOs
{
    public class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
