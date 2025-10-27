using SGS.TaskTracker.Core.Entities;
using System.Security.Claims;

namespace SGS.TaskTracker.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
