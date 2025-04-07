using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces.IRepositories;
using QuizApp.QuizApp.Infrastructure.Data;

namespace QuizApp.QuizApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.UserName == username);
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            _dbContext.RefreshTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
            if (refreshToken != null)
            {
                refreshToken.Revoked = DateTime.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
                _dbContext.RefreshTokens.Update(refreshToken);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
