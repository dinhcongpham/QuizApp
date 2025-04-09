using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [Column("email")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(255, ErrorMessage = "Password hash cannot exceed 255 characters.")]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = "";

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = "";

        [Required]
        [Column("role")]
        public string Role { get; set; } = "User"; // User | Admin

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
