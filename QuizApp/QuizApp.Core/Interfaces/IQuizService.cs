using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    // IQuizRepository.cs
    public interface IQuizService
    {
        Task<List<QuizResponseDto>> GetAllAsync(int userId);
        Task<QuizResponseDto?> GetByIdAsync(int id);
        Task<QuizResponseDto> CreateAsync(CreateQuizDto quizDto);
        Task<bool> UpdateAsync(int id, CreateQuizDto updatedQuizDto);
        Task<bool> DeleteAsync(int id);
    }
}
