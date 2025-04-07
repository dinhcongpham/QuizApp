using QuizApp.QuizApp.Core.Entities;

namespace QuizApp.QuizApp.Core.Interfaces.IRepositories
{
    public interface IQuizRepository : IGenericRepository<Quiz>
    {
        Task<Quiz> GetByIdWithQuestionsAsync(int id);
        Task<Quiz> GetByAccessCodeAsync(string accessCode);
        Task<IEnumerable<Quiz>> GetByCreatorIdAsync(string creatorId);
        Task<IEnumerable<Quiz>> GetPublicQuizzesAsync(int page, int pageSize);
        Task<Question> GetQuestionByIdAsync(int id);
        Task<Option> GetOptionByIdAsync(int id);
    }
}
