using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = "";

        [StringLength(50)]
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "User";

    }

    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }

    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryDay { get; set; }
    }

    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = "";
    }

    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }

    public class ResetPasswordDto
    {
        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string Token { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
    }
}
