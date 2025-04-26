using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class GameRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public int HostUserId { get; set; }
        public List<QuestionResponseDto> Questions { get; set; } = new();
        public List<RoomParticipantDto> Participants { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime StartedAt { get; set; }
    }
    
    public class GameStateDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int CurrentQuestionIndex { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UserAnswerEntryDto
    {
        public int UserId { get; set; } = 0;
        public int RoomId { get; set; } = 0;
        public int QuestionId { get; set; } = 0;
        public string SelectedOption { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
        public int Score { get; set; } = 0;
        public decimal TimeTaken { get; set; } = 0;
    }

    public class UserAnswersDto
    {
        public int RoomId { get; set; } = 0;
        public List<UserAnswerEntryDto> UserAnswers { get; set; } = new();
    }

    public class LeaderboardSnapshotEntryDto
    {
        public int RoomId { get; set; } = 0;
        public int QuizId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int Score { get; set; } = 0;

    }

    public class LeaderboardSnapshotDto
    {
        public int RoomId { get; set; } = 0;
        public List<LeaderboardSnapshotEntryDto> Entries { get; set; } = new();
    }

    public class RoomParticipantDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}