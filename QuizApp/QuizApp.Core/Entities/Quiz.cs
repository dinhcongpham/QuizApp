using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("quizs")]
    public class Quiz
    {
        [Key]
        [Column("quiz_id")]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; } = "";

        [Required]
        [Column("description")]
        public string Description { get; set; } = "";

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("owner_id")]
        public int OwnerId { get; set; }

        // Navigation property for the owner of the quiz

        public User? Owner { get; set; }

        public List<Question> Questions { get; set; } = new();
        public List<Room> Rooms { get; set; } = new();
    }
}
