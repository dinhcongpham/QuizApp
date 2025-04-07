using System;
using System.Collections.Generic;

namespace QuizApp.QuizApp.Core.Entities
{
    public class ApplicationUser 
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public ICollection<Quiz>? CreatedQuizzes { get; set; }
        public ICollection<QuizAttempt>? QuizAttempts { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }

    public class RefreshToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; } = "";
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; } = "";
        public string ReplacedByToken { get; set; } = "";
        public bool IsActive => Revoked == null && !IsExpired;
        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }   
    }
}
