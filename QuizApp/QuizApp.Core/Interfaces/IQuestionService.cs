using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IQuestionService
    {
        Task<QuestionResponseDto> CreateAsync(CreateQuestionDto dto);
        Task<IEnumerable<QuestionResponseDto>> GetByQuizIdAsync(int quizId);
        Task<QuestionResponseDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateQuestionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
