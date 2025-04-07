using QuizApp.QuizApp.Core.Entities;

namespace QuizApp.QuizApp.Core.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<ApplicationUser> GetByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task AddRefreshTokenAsync(RefreshToken token);
        Task UpdateRefreshTokenAsync(RefreshToken token);
        Task RevokeRefreshTokenAsync(string token, string ipAddress);
    }
}
