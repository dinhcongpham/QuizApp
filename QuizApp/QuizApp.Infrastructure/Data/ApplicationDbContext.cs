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
        public DbSet<QuizSession> QuizSessions { get; set; }
        public DbSet<UserQuizResult> UserQuizResults { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User - RefreshToken: One to Many
            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Quiz: One to Many
            builder.Entity<Quiz>()
                .HasOne(q => q.Owner)
                .WithMany()
                .HasForeignKey(q => q.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quiz - Question: One to Many
            builder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quiz - QuizSession: One to Many
            builder.Entity<QuizSession>()
                .HasOne(qs => qs.Quiz)
                .WithMany()
                .HasForeignKey(qs => qs.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - QuizSession (Host)
            builder.Entity<QuizSession>()
                .HasOne(qs => qs.HostUser)
                .WithMany()
                .HasForeignKey(qs => qs.HostUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // QuizSession - UserQuizResult: One to Many
            builder.Entity<UserQuizResult>()
                .HasOne(r => r.Session)
                .WithMany()
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - UserQuizResult: One to Many
            builder.Entity<UserQuizResult>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserQuizResult - UserAnswer: One to Many
            builder.Entity<UserAnswer>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question - UserAnswer: One to Many
            builder.Entity<UserAnswer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
