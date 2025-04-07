namespace QuizApp.QuizApp.Core.Entities
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string AccessCode { get; set; } = "";
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CreatorId { get; set; } = "";
        public ApplicationUser? Creator { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<QuizSession>? Sessions { get; set; }
        public ICollection<QuizAttempt>? Attempts { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
        public int Points { get; set; } = 1;
        public int TimeLimit { get; set; } = 30; // Time limit in seconds
        public int DisplayOrder { get; set; }
        public QuestionType Type { get; set; }
        public ICollection<Option>? Options { get; set; }
    }

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        ShortAnswer
    }

    public class Option
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
    }

    public class QuizSession
    {
        public int Id { get; set; }
        public string SessionCode { get; set; } = "";
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
        public string HostId { get; set; } = "";
        public ApplicationUser? Host { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public SessionStatus Status { get; set; }
        public ICollection<SessionParticipant>? Participants { get; set; }
    }

    public enum SessionStatus
    {
        Waiting,
        InProgress,
        Completed,
        Cancelled
    }

    public class SessionParticipant
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public QuizSession? Session { get; set; }
        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }
        public string DisplayName { get; set; } = "";
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public int TotalScore { get; set; } = 0;
    }

    public class QuizAttempt
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
        public string UserId { get; set; } = "";
        public ApplicationUser? User { get; set; }
        public int? SessionId { get; set; }
        public QuizSession? Session { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Score { get; set; }
        public ICollection<QuestionResponse>? Responses { get; set; }
    }

    public class QuestionResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        public int AttemptId { get; set; }
        public QuizAttempt? Attempt { get; set; }
        public int? SelectedOptionId { get; set; }
        public Option? SelectedOption { get; set; }
        public string TextResponse { get; set; } = "";
        public DateTime RespondedAt { get; set; }
        public int AwardedPoints { get; set; }
        public TimeSpan ResponseTime { get; set; }
    }
}
