using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokens(int userId);
        public string GenerateAccessToken(int userId);
        public string GenerateRefreshToken();
        Task StoreRefreshToken(int userId, string refreshToken);
    }
}
