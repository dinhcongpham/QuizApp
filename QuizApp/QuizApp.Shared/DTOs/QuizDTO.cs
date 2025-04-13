using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
    public class CreateQuizDto
    {
        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        public int OwnerId { get; set; } 
    }

    // (for return)
    public class QuizResponseDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
