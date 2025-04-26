using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QuizApp.QuizApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Quiz> Quizs { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomParticipant> RoomParticipants { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<LeaderboardSnapshot> LeaderboardSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USERS
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // REFRESH TOKENS
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId)
                .HasDatabaseName("idx_refresh_tokens_user");

            // QUIZ
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Owner)
                .WithMany(u => u.Quizs)
                .HasForeignKey(q => q.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // QUESTIONS
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasIndex(q => q.QuizId)
                .HasDatabaseName("idx_questions_quiz_id");

            // ROOMS
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Quiz)
                .WithMany(q => q.Rooms)
                .HasForeignKey(r => r.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.HostUser)
                .WithMany(u => u.HostedRooms)
                .HasForeignKey(r => r.HostUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasIndex(r => r.QuizId)
                .HasDatabaseName("idx_rooms_quiz_id");

            // USER ANSWERS
            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAnswers)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Room)
                .WithMany(r => r.UserAnswers)
                .HasForeignKey(ua => ua.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAnswer>()
                .HasIndex(ua => new { ua.RoomId, ua.QuestionId })
                .HasDatabaseName("idx_user_answers_room_question");

            modelBuilder.Entity<UserAnswer>()
                .HasIndex(ua => new { ua.UserId, ua.RoomId })
                .HasDatabaseName("idx_user_answers_user_room");

            // LEADERBOARD
            modelBuilder.Entity<LeaderboardSnapshot>()
                .HasOne(lb => lb.User)
                .WithMany(u => u.LeaderboardSnapshots)
                .HasForeignKey(lb => lb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaderboardSnapshot>()
                .HasOne(lb => lb.Room)
                .WithMany(r => r.LeaderboardSnapshots)
                .HasForeignKey(lb => lb.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaderboardSnapshot>()
                .HasIndex(lb => new { lb.RoomId, lb.Score })
                .HasDatabaseName("idx_leaderboard_room_score");

            // ROOM PARTICIPANTS
            modelBuilder.Entity<RoomParticipant>()
                .HasOne(rp => rp.User)
                .WithMany(u => u.RoomParticipants)
                .HasForeignKey(rp => rp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomParticipant>()
                .HasOne(rp => rp.Room)
                .WithMany(r => r.Participants)
                .HasForeignKey(rp => rp.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomParticipant>()
                .HasIndex(rp => new { rp.RoomId, rp.UserId })
                .HasDatabaseName("idx_participants_room_user");
        }

    }
}
