using QuizApp.QuizApp.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class SessionDto
    {
        public int Id { get; set; }
        public string SessionCode { get; set; } = "";
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = "";
        public string HostId { get; set; } = "";
        public string HostName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public SessionStatus Status { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public int ParticipantCount { get; set; }
    }

    public class CreateSessionDto
    {
        [Required]
        public int QuizId { get; set; }
    }

    public class JoinSessionDto
    {
        [Required]
        [StringLength(8)]
        public string SessionCode { get; set; } = "";

        [StringLength(50)]
        public string DisplayName { get; set; } = "";
    }

    public class ParticipantDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string UserId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public int TotalScore { get; set; }
    }
}
