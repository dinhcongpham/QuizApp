using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Infrastructure.Data;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Add(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task<RefreshToken?> GetByToken(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }
        public async Task Delete(string token)
        {
            var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RefreshToken?> GetByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == userId);
        }
    }
}
