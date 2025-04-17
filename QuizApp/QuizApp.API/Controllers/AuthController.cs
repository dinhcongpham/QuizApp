using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
            {
                return Conflict("User already exists");
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            return Ok(result);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userRepository.ChangePasswordAsync(changePasswordDto);
            if (!result)
            {
                return BadRequest("Failed to update password");
            }
            return Ok(result);
        }

        [HttpPost("update-username")]
        public async Task<IActionResult> UpdateUsername([FromBody] ChangeUserNameDto changeUserNameDto)
        {
            var result = await _userRepository.ChangeUserNameAsync(changeUserNameDto);
            if (!result)
            {
                return BadRequest("Failed to update username");
            }
            return Ok(result);
        }
    }
}
