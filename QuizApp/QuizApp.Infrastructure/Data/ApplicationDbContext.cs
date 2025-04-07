using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QuizApp.QuizApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<ApplicationUser>? Users { get; set; }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<QuizSession> QuizSessions { get; set; }
        public DbSet<SessionParticipant> SessionParticipants { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuestionResponse> QuestionResponses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure entity relationships
            builder.Entity<Quiz>()
                .HasOne(q => q.Creator)
                .WithMany(u => u.CreatedQuizzes)
                .HasForeignKey(q => q.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.QuizId);

            builder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId);

            builder.Entity<QuizSession>()
                .HasOne(s => s.Quiz)
                .WithMany(q => q.Sessions)
                .HasForeignKey(s => s.QuizId);

            builder.Entity<QuizSession>()
                .HasOne(s => s.Host)
                .WithMany()
                .HasForeignKey(s => s.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SessionParticipant>()
                .HasOne(p => p.Session)
                .WithMany(s => s.Participants)
                .HasForeignKey(p => p.SessionId);

            builder.Entity<SessionParticipant>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuizAttempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId);

            builder.Entity<QuizAttempt>()
                .HasOne(a => a.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuestionResponse>()
                .HasOne(r => r.Question)
                .WithMany()
                .HasForeignKey(r => r.QuestionId);

            builder.Entity<QuestionResponse>()
                .HasOne(r => r.Attempt)
                .WithMany(a => a.Responses)
                .HasForeignKey(r => r.AttemptId);

            builder.Entity<RefreshToken>()
                .HasOne(t => t.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(t => t.UserId);
        }
    }
}
