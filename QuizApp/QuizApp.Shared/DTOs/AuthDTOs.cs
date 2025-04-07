using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = "";
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }

    public class AuthResponseDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime TokenExpires { get; set; }
    }

    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = "";
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
