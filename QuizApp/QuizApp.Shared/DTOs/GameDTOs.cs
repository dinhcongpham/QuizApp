using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class GameRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public int HostUserId { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
        public List<RoomParticipantDto> Participants { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class GameStateDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int CurrentQuestionIndex { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AnswerResultDto
    {
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public decimal Score { get; set; }
    }

    public class LeaderboardEntryDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal TimeTaken { get; set; }
    }

    public class LeaderboardSnapshotDto
    {
        public int RoomId { get; set; }
        public int QuestionId { get; set; }
        public List<LeaderboardEntryDto> Entries { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class GameResultsDto
    {
        public int RoomId { get; set; }
        public List<LeaderboardEntryDto> FinalLeaderboard { get; set; } = new();
        public List<QuestionResultDto> QuestionResults { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string CorrectOption { get; set; } = string.Empty;
        public List<UserAnswerDto> UserAnswers { get; set; } = new();
    }

    public class UserAnswerDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string SelectedOption { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public decimal TimeTaken { get; set; }
        public decimal Score { get; set; }
    }

    public class RoomParticipantDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
} 