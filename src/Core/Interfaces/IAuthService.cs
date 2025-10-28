using SGS.TaskTracker.Core.DTOs;

namespace SGS.TaskTracker.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(UserRegisterRequest request);
        Task<AuthResponse> LoginAsync(UserLoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(TokenRefreshRequest request);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(int userId);
    }
}
