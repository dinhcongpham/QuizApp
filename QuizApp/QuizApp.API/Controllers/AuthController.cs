using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Attempting to register user with email: {Email}", registerDto.Email);
                var result = await _authService.RegisterAsync(registerDto);
                if (result == null)
                {
                    _logger.LogWarning("Registration failed - User already exists with email: {Email}", registerDto.Email);
                    return Conflict("User already exists");
                }
                _logger.LogInformation("User registered successfully with email: {Email}", registerDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with email: {Email}", registerDto.Email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Attempting to login user with email: {Email}", loginDto.Email);
                var result = await _authService.LoginAsync(loginDto);
                _logger.LogInformation("User logged in successfully with email: {Email}", loginDto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user with email: {Email}", loginDto.Email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                _logger.LogInformation("Attempting to refresh token");
                var result = await _authService.RefreshTokenAsync(refreshTokenDto);
                _logger.LogInformation("Token refreshed successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while refreshing token");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update password for user: {UserId}", changePasswordDto.UserId);
                var result = await _userRepository.ChangePasswordAsync(changePasswordDto);
                if (!result)
                {
                    _logger.LogWarning("Failed to update password for user: {UserId}", changePasswordDto.UserId);
                    return BadRequest("Failed to update password");
                }
                _logger.LogInformation("Password updated successfully for user: {UserId}", changePasswordDto.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating password for user: {UserId}", changePasswordDto.UserId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("update-username")]
        [Authorize]
        public async Task<IActionResult> UpdateUsername([FromBody] ChangeUserNameDto changeUserNameDto)
        {
            try
            {
                _logger.LogInformation("Attempting to update username for user: {UserId}", changeUserNameDto.UserId);
                var result = await _userRepository.ChangeUserNameAsync(changeUserNameDto);
                if (!result)
                {
                    _logger.LogWarning("Failed to update username for user: {UserId}", changeUserNameDto.UserId);
                    return BadRequest("Failed to update username");
                }
                _logger.LogInformation("Username updated successfully for user: {UserId}", changeUserNameDto.UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating username for user: {UserId}", changeUserNameDto.UserId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Attempting to send password reset email to: {Email}", request.Email);
                var can = await _authService.ForgotPasswordAsync(request);
                if (can == false)
                {
                    _logger.LogWarning("No user found with email: {Email}", request.Email);
                    return NotFound("User not found");
                }
                // Logic to send password reset email goes here
                _logger.LogInformation("Password reset email sent to: {Email}", request.Email);
                return Ok("Password reset email sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending password reset email to: {Email}", request.Email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
