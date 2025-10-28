using Microsoft.EntityFrameworkCore;
using SGS.TaskTracker.Core.Data;
using SGS.TaskTracker.Core.Entities;
using SGS.TaskTracker.Interfaces;

namespace SGS.TaskTracker.Infrastructure.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TaskTrackerContext _context;

        public RefreshTokenRepository(TaskTrackerContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                _context.RefreshTokens.Update(refreshToken);
            }
        }

        public async Task RevokeAllTokensForUserAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            _context.RefreshTokens.UpdateRange(tokens);
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            return await _context.RefreshTokens
                .AnyAsync(rt => rt.Token == token && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
