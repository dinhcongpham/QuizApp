using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class UserStatsDTO
    {
        public int TotalQuizzesCreated { get; set; }
        public int TotalQuizzesParticipated { get; set; }
        public double AccuracyRate { get; set; }
    }

    public class DetailUserAnswersDTO
    {
        public string Content { get; set; } = "";
        public string OptionA { get; set; } = "";
        public string OptionB { get; set; } = "";
        public string OptionC { get; set; } = "";
        public string OptionD { get; set; } = "";
        public string CorrectOption { get; set; } = "";
        public string SelectedOption { get; set; } = "";
    }

    public class DetailQuizStatsDTO
    {
        public int RoomId { get; set; }
        public DateTime StartedAt { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<DetailUserAnswersDTO> UserAnswers { get; set; }
    }

    public class QuizStatsDTO
    {
        public List<DetailQuizStatsDTO> Quiz { get; set; }
    }

    public class QuizRoomRawEntry
    {
        public int RoomId { get; set; }
        public DateTime StartedAt { get; set; }
        public string QuizTitle { get; set; } = "";
        public string QuizDescription { get; set; } = "";

        public string Content { get; set; } = "";
        public string OptionA { get; set; } = "";
        public string OptionB { get; set; } = "";
        public string OptionC { get; set; } = "";
        public string OptionD { get; set; } = "";
        public string CorrectOption { get; set; } = "";
        public string SelectedOption { get; set; } = "";
    }
}