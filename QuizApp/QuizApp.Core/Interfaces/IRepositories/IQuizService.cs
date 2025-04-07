using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces.IRepositories
{
    public interface IQuizService
    {
        Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto, string userId);
        Task<QuizDto> UpdateQuizAsync(int id, UpdateQuizDto updateQuizDto, string userId);
        Task DeleteQuizAsync(int id, string userId);
        Task<QuizDto> GetQuizByIdAsync(int id);
        Task<IEnumerable<QuizDto>> GetQuizzesByUserIdAsync(string userId);
        Task<QuizDto> GetQuizByAccessCodeAsync(string accessCode);
        Task<IEnumerable<QuizDto>> GetPublicQuizzesAsync(int page, int pageSize);
        Task<QuestionDto> AddQuestionAsync(int quizId, CreateQuestionDto questionDto, string userId);
        Task<QuestionDto> UpdateQuestionAsync(int questionId, UpdateQuestionDto questionDto, string userId);
        Task DeleteQuestionAsync(int questionId, string userId);
    }
}
