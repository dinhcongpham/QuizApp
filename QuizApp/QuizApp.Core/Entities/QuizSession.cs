using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("quiz_sessions")]
    public class QuizSession
    {
        [Key]
        [Column("session_id")]
        public int Id { get; set; }

        [Required]
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Required]
        [Column("room_code")]
        public string RoomCode { get; set; } = "";

        [Required]
        [Column("host_user_id")]
        public int HostUserId { get; set; }

        [Required]
        [Column("start_time")]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; } = false;


        // Navigation properties

        public Quiz? Quiz { get; set; }
        public User? HostUser { get; set; }
    }
}
