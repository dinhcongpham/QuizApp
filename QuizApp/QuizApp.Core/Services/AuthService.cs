using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces.IRepositories;
using QuizApp.QuizApp.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuizApp.QuizApp.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, string ipAddress)
        {
            // Check if user exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
                throw new Exception("Email is already registered");

            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
                throw new Exception("Username is already taken");

            // Create new user
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Username,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // TODO: Send email confirmation

            // Generate JWT token and refresh token
            return await GenerateAuthResponseAsync(user, ipAddress);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string ipAddress)
        {
            // Find user by username or email
            var user = await _userManager.FindByNameAsync(loginDto.Username) ??
                       await _userManager.FindByEmailAsync(loginDto.Username);

            if (user == null)
                throw new Exception("Invalid username or password");

            // Verify password
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
                throw new Exception("Invalid username or password");

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate JWT token and refresh token
            return await GenerateAuthResponseAsync(user, ipAddress);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(token);

            if (refreshToken == null)
                throw new Exception("Invalid token");

            if (!refreshToken.IsActive)
                throw new Exception("Token expired or revoked");

            if (refreshToken.User == null)
                throw new Exception("User not found");

            // Revoke current refresh token
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _userRepository.UpdateRefreshTokenAsync(refreshToken);

            // Generate new tokens
            return await GenerateAuthResponseAsync(refreshToken.User, ipAddress);
        }

        public async Task RevokeTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(token);

            if (refreshToken == null)
                throw new Exception("Invalid token");

            if (!refreshToken.IsActive)
                throw new Exception("Token expired or revoked");

            // Revoke refresh token
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _userRepository.UpdateRefreshTokenAsync(refreshToken);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return; // Don't reveal that user doesn't exist

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // TODO: Send password reset email
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            return result.Succeeded;
        }

        public async Task<bool> VerifyEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user, string ipAddress)
        {
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);

            // Save refresh token
            user.RefreshTokens ??= new List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email,
                Token = jwtToken.Token,
                TokenExpires = jwtToken.Expires,
                RefreshToken = refreshToken.Token
            };
        }

        private (string Token, DateTime Expires) GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "what the fuck");
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expires);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"])),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}
