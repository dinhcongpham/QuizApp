using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface ITokenService
    {
        public AuthResponseDto GenerateTokens(int userId);
        public string GenerateAccessToken(int userId);
        public string GenerateRefreshToken();
        public void StoreRefreshToken(int userId, string refreshToken);
    }
}
