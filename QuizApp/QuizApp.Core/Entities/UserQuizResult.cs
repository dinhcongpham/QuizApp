using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("user_quiz_results")]
    public class UserQuizResult
    {
        [Key]
        [Column("result_id")]
        public int Id { get; set; }

        [Required]
        [Column("session_id")]
        public int SessionId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("score")]
        public string Score { get; set; } = "0";

        [Required]
        [Column("submitted_at")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;


        // Navigation properties

        public QuizSession? Session { get; set; }
        public User? User { get; set; }
    }

}
