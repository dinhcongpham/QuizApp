using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("rooms")]
    public class Room
    {
        [Key]
        [Column("room_id")]
        public int Id { get; set; }

        [Required]
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Required]
        [Column("host_user_id")]
        public int HostUserId { get; set; }

        [Required]
        [Column("room_code")]
        public string RoomCode { get; set; } = string.Empty;

        [Required]
        [Column("started_at")]
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        [Column("ended_at")]
        public DateTime? EndedAt { get; set; }

        public Quiz? Quiz { get; set; }
        public User? HostUser { get; set; }
        public ICollection<UserAnswer>? UserAnswers { get; set; }
        public ICollection<LeaderboardSnapshot>? LeaderboardSnapshots { get; set; }
        public ICollection<RoomParticipant>? Participants { get; set; }
    }
}
