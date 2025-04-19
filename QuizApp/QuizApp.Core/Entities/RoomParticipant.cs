using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("room_participants")]
    public class RoomParticipant
    {
        [Key]
        [Column("room_participant_id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("room_id")]
        public int RoomId { get; set; }

        [Required]
        [Column("joined_at")]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Room? Room { get; set; }
    }
}
