using Microsoft.IdentityModel.Tokens;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuizApp.QuizApp.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;

        public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDto> GenerateTokens(int userId)
        {
            // Generate access token
            var accessToken =  GenerateAccessToken(userId);

            // Generate refresh token
            var refreshToken =  GenerateRefreshToken();

            // Store refresh token in database (important for security!)
            await StoreRefreshToken(userId, refreshToken);

            var user = await _userRepository.GetByIdAsync(userId);

            return new AuthResponseDto
            {
                UserId = userId,
                FullName = user.FullName, 
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryDay = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]))
            };
        }

        public string GenerateAccessToken(int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task StoreRefreshToken(int userId, string refreshToken)
        {
            // Store in database with userId, refreshToken, creation time, expiry time
            // This is just a placeholder - implement your database logic here
            await _refreshTokenRepository.Add(new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]))
            });
        }
    }
}
