using SGS.TaskTracker.Core.Entities;

namespace SGS.TaskTracker.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        void Update(RefreshToken refreshToken);
        Task RevokeTokenAsync(string token);
        Task RevokeAllTokensForUserAsync(int userId);
        Task<bool> IsTokenValidAsync(string token);
        Task<int> SaveChangesAsync();
    }
}
