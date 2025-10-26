using SGS.TaskTracker.Models;
using System.Security.Claims;

namespace SGS.TaskTracker.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
