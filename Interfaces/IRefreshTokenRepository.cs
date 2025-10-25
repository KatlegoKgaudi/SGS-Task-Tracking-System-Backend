using SGS.TaskTracker.Models;

namespace SGS.TaskTracker.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetValidTokenAsync(int userId, string token);
        Task AddAsync(RefreshToken refreshToken);
        Task RevokeAsync(int tokenId);
        Task RevokeAllForUserAsync(int userId);
    }
}
