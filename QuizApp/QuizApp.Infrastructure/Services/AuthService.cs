using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;
using QuizApp.QuizApp.Shared.Helpers.PasswordHash;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuizApp.QuizApp.Infrastructure.Services
{
    public class AuthService : IAuthService // Inherit from ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
                return null;

            var user = await _userRepository.CreateUser(registerDto);

            if (user == null)
                return null;

            // Generate tokens
            var tokenResponse = await _tokenService.GenerateTokens(user.Id);

            return tokenResponse;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Validate user credentials
            var user = await _userRepository.Authenticate(loginDto.Email, loginDto.Password);

            if (user == null)
                throw new ArgumentException("Invalid username or password");

            var existingTokens = await _refreshTokenRepository.GetByUserIdAsync(user.Id);
            if (existingTokens != null)
            {
                await _refreshTokenRepository.Delete(existingTokens.Token);
            }

            // Generate tokens
            var tokenResponse = await _tokenService.GenerateTokens(user.Id);

            return tokenResponse;
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            // Validate refresh token (check if it exists and isn't expired)
            var refreshToken = await _refreshTokenRepository.GetByToken(refreshTokenDto.RefreshToken);

            if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new ArgumentException("Invalid refresh token");

            // Invalidate old refresh token (optional but recommended)
            await _refreshTokenRepository.Delete(refreshTokenDto.RefreshToken);

            // Generate new tokens
            var tokenResponse = await _tokenService.GenerateTokens(refreshToken.UserId);

            return tokenResponse;
        }
    }
}
