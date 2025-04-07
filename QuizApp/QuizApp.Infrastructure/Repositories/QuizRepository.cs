using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces.IRepositories;
using QuizApp.QuizApp.Infrastructure.Data;

namespace QuizApp.QuizApp.Infrastructure.Repositories
{
    public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
    {
        public QuizRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Quiz> GetByIdWithQuestionsAsync(int id)
        {
            return await _dbContext.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quiz> GetByAccessCodeAsync(string accessCode)
        {
            return await _dbContext.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.AccessCode == accessCode);
        }

        public async Task<IEnumerable<Quiz>> GetByCreatorIdAsync(string creatorId)
        {
            return await _dbContext.Quizzes
                .Where(q => q.CreatorId == creatorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetPublicQuizzesAsync(int page, int pageSize)
        {
            return await _dbContext.Quizzes
                .Where(q => q.IsPublic)
                .OrderByDescending(q => q.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            return await _dbContext.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Option> GetOptionByIdAsync(int id)
        {
            return await _dbContext.Options.FindAsync(id);
        }
    }
}
