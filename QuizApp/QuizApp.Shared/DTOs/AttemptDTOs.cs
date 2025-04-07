using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class AttemptDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = "";
        public string UserId { get; set; } = "";
        public int? SessionId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CompletedQuestions { get; set; }
    }

    public class StartAttemptDto
    {
        [Required]
        public int QuizId { get; set; }

        public int? SessionId { get; set; }
    }

    public class SubmitAnswerDto
    {
        [Required]
        public int AttemptId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public int? SelectedOptionId { get; set; }

        public string TextResponse { get; set; } = "";

        [Required]
        public TimeSpan ResponseTime { get; set; }
    }

    public class AttemptResultDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = "";
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Score { get; set; }
        public int TotalPoints { get; set; }
        public double ScorePercentage { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new List<QuestionResultDto>();
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public int Points { get; set; }
        public int AwardedPoints { get; set; }
        public int? SelectedOptionId { get; set; }
        public string TextResponse { get; set; } = "";
        public bool IsCorrect { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }
}
