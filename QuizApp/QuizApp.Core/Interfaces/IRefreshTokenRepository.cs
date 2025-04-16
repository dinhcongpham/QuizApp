using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        public Task Add(RefreshToken refreshTokenDto);
        public Task<RefreshToken?> GetByToken(string token);
        public Task Delete(string token);
        public Task<RefreshToken> GetByUserIdAsync(int userId);
    }
}
