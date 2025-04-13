using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class CreateQuestionDto
    {
        [Required]
        public int QuizId { get; set; }

        [Required]
        public string Content { get; set; } = "";

        [Required]
        public string OptionA { get; set; } = "";

        [Required]
        public string OptionB { get; set; } = "";

        [Required]
        public string OptionC { get; set; } = "";

        [Required]
        public string OptionD { get; set; } = "";

        [Required]
        [RegularExpression("A|B|C|D", ErrorMessage = "Correct option must be one of A, B, C, D.")]
        public string CorrectOption { get; set; } = "";
    }

    public class UpdateQuestionDto
    {
        [Required]
        public string Content { get; set; } = "";

        [Required]
        public string OptionA { get; set; } = "";

        [Required]
        public string OptionB { get; set; } = "";

        [Required]
        public string OptionC { get; set; } = "";

        [Required]
        public string OptionD { get; set; } = "";

        [Required]
        [RegularExpression("A|B|C|D", ErrorMessage = "Correct option must be one of A, B, C, D.")]
        public string CorrectOption { get; set; } = "";
    }

    public class QuestionResponseDto
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string Content { get; set; } = "";
        public string OptionA { get; set; } = "";
        public string OptionB { get; set; } = "";
        public string OptionC { get; set; } = "";
        public string OptionD { get; set; } = "";
        public string CorrectOption { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}