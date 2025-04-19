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
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("role")]
        public string Role { get; set; } = string.Empty;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<Quiz>? Quizs { get; set; }
        public ICollection<Room>? HostedRooms { get; set; }
        public ICollection<UserAnswer>? UserAnswers { get; set; }
        public ICollection<LeaderboardSnapshot>? LeaderboardSnapshots { get; set; }
        public ICollection<RoomParticipant>? RoomParticipants { get; set; }
    }
}
