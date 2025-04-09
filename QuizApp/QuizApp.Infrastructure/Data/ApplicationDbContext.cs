using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QuizApp.QuizApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<User>? Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
