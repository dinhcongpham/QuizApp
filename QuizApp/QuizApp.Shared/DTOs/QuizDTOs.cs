using QuizApp.QuizApp.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string AccessCode { get; set; } = "";
        public bool IsPublic { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatorId { get; set; } = "";
        public string CreatorName { get; set; } = "";
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }

    public class CreateQuizDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = "";

        [StringLength(500)]
        public string Description { get; set; } = "";

        [Required]
        public bool IsPublic { get; set; }
    }

    public class UpdateQuizDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = "";

        [StringLength(500)]
        public string Description { get; set; } = "";

        [Required]
        public bool IsPublic { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public int QuizId { get; set; }
        public int Points { get; set; }
        public int TimeLimit { get; set; }
        public int DisplayOrder { get; set; }
        public QuestionType Type { get; set; }
        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }

    public class CreateQuestionDto
    {
        [Required]
        [StringLength(500)]
        public string Text { get; set; } = "";

        public int Points { get; set; } = 1;

        [Range(5, 300)]
        public int TimeLimit { get; set; } = 30;

        [Required]
        public QuestionType Type { get; set; }

        [Required]
        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();
    }

    public class UpdateQuestionDto
    {
        [Required]
        [StringLength(500)]
        public string Text { get; set; } = "";

        public int Points { get; set; } = 1;

        [Range(5, 300)]
        public int TimeLimit { get; set; } = 30;

        [Required]
        public QuestionType Type { get; set; }

        [Required]
        public List<UpdateOptionDto> Options { get; set; } = new List<UpdateOptionDto>();
    }

    public class OptionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }
    }

    public class CreateOptionDto
    {
        [Required]
        [StringLength(200)]
        public string Text { get; set; } = "";

        [Required]
        public bool IsCorrect { get; set; }
    }

    public class UpdateOptionDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Text { get; set; } = "";

        [Required]
        public bool IsCorrect { get; set; }
    }
}
