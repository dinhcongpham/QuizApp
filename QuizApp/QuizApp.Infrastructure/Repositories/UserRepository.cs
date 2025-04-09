using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Infrastructure.Data;
using QuizApp.QuizApp.Shared.DTOs;
using QuizApp.QuizApp.Shared.Helpers.PasswordHash;

namespace QuizApp.QuizApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepository(ApplicationDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> CreateUser(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                throw new ArgumentNullException(nameof(registerDto));
            }
            var passwordHash = _passwordHasher.HashPassword(registerDto.Password);

            // Create new user
            var user = new User
            {
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = await _dbContext.Users
                         .AsNoTracking() // Read-only operation, improves performance
                         .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyHashedPassword(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task<int> UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteUser(Guid userId)
        {
            var user = await GetByIdAsync(userId.ToString());
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                return await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("User not found");
            }
        }

    }
}
