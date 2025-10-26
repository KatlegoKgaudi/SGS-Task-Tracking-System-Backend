using System.ComponentModel.DataAnnotations;

namespace SGS.TaskTracker.Dtos
{
    public class UserLoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
