using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> CreateUser(RegisterDto registerDto);
        Task<User?> Authenticate(string email, string password);
        Task<bool> EmailExistsAsync(string email);
        Task<int> UpdateUser(User user);
        Task<int> DeleteUser(Guid userId);
    }
}
