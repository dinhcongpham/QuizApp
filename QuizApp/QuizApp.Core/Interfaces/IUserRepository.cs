using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> CreateUser(RegisterDto registerDto);
        Task<User?> Authenticate(string email, string password);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<bool> ChangeUserNameAsync(ChangeUserNameDto changeUserNameDto);
        Task<bool> UpdateUserAsync(User user);
        Task<int> DeleteUser(int userId);
    }
}
