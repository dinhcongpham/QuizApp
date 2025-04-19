using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("questions")]
    public class Question
    {
        [Key]
        [Column("question_id")]
        public int Id { get; set; }

        [Required]
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; } = "";

        [Required]
        [Column("option_a")]
        public string OptionA { get; set; } = "";

        [Required]
        [Column("option_b")]
        public string OptionB { get; set; } = "";

        [Required]
        [Column("option_c")]
        public string OptionC { get; set; } = "";

        [Required]
        [Column("option_d")]
        public string OptionD { get; set; } = "";

        [Required]
        [Column("correct_option")]
        public string CorrectOption { get; set; } = "";

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navigation property for the quiz this question belongs to

        public Quiz? Quiz { get; set; }
        public List<UserAnswer> UserAnswers { get; set; } = new();
    }
}
