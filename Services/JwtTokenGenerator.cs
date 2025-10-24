using SGS.TaskTracker.Interfaces;
using SGS.TaskTracker.Models;
using System.Security.Claims;
using System.Text;

namespace SGS.TaskTracker.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        //todo - define JWT Token Generator logic
        public JwtTokenGenerator() { }

        public string GenerateAccessToken(User user)
        {
            throw new NotImplementedException();
        }

        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}