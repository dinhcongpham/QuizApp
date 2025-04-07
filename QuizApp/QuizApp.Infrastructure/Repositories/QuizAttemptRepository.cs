using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces.IRepositories;
using QuizApp.QuizApp.Infrastructure.Data;

namespace QuizApp.QuizApp.Infrastructure.Repositories
{
    public class QuizAttemptRepository : GenericRepository<QuizAttempt>, IQuizAttemptRepository
    {
        public QuizAttemptRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<QuizAttempt> GetByIdWithResponsesAsync(int id)
        {
            return await _dbContext.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.Responses)
                    .ThenInclude(r => r.Question)
                .Include(a => a.Responses)
                    .ThenInclude(r => r.SelectedOption)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<QuizAttempt>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.QuizAttempts
                .Include(a => a.Quiz)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizAttempt>> GetByQuizIdAsync(int quizId)
        {
            return await _dbContext.QuizAttempts
                .Include(a => a.User)
                .Where(a => a.QuizId == quizId)
                .OrderByDescending(a => a.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizAttempt>> GetBySessionIdAsync(int sessionId)
        {
            return await _dbContext.QuizAttempts
                .Include(a => a.User)
                .Where(a => a.SessionId == sessionId)
                .OrderByDescending(a => a.Score)
                .ToListAsync();
        }

        public async Task<QuizAttempt> GetActiveAttemptAsync(string userId, int quizId)
        {
            return await _dbContext.QuizAttempts
                .Include(a => a.Responses)
                .FirstOrDefaultAsync(a => a.UserId == userId && a.QuizId == quizId && a.CompletedAt == null);
        }

        public async Task AddQuestionResponseAsync(QuestionResponse response)
        {
            await _dbContext.QuestionResponses.AddAsync(response);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateQuestionResponseAsync(QuestionResponse response)
        {
            _dbContext.QuestionResponses.Update(response);
            await _dbContext.SaveChangesAsync();
        }
    }
}
