using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Core.Entities
{
    [Table("leaderboard_snapshots")]
    public class LeaderboardSnapshot
    {
        [Key]
        [Column("leaderboard_snapshot_id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("room_id")]
        public int RoomId { get; set; }

        [Required]
        [Column("score")]
        public decimal Score { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Room? Room { get; set; }
    }
}
