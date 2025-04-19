using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("user_answers")]
    public class UserAnswer
    {
        [Key]
        [Column("answer_id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("room_id")]
        public int RoomId { get; set; }

        [Required]
        [Column("question_id")]
        public int QuestionId { get; set; }

        [Required]
        [Column("selected_option")]
        public string SelectedOption { get; set; } = string.Empty;

        [Required]
        [Column("is_correct")]
        public bool IsCorrect { get; set; }

        [Required]
        [Column("answered_at")]
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("time_taken_seconds")]
        public decimal TimeTakenSeconds { get; set; }

        public User? User { get; set; }
        public Room? Room { get; set; }
        public Question? Question { get; set; }
    }
}
