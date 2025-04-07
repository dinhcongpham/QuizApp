using QuizApp.QuizApp.Core.Entities;

namespace QuizApp.QuizApp.Core.Interfaces.IRepositories
{
    public interface IQuizAttemptRepository : IGenericRepository<QuizAttempt>
    {
        Task<QuizAttempt> GetByIdWithResponsesAsync(int id);
        Task<IEnumerable<QuizAttempt>> GetByUserIdAsync(string userId);
        Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(int quizId);
        Task<IEnumerable<QuizAttempt>> GetBySessionIdAsync(int sessionId);
        Task<QuizAttempt> GetActiveAttemptAsync(string userId, int quizId);
        Task AddQuestionResponseAsync(QuestionResponse response);
        Task UpdateQuestionResponseAsync(QuestionResponse response);
    }
}
