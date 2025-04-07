using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Infrastructure.Data;

namespace QuizApp.QuizApp.Infrastructure.Repositories
{
    public class QuizSessionRepository : GenericRepository<QuizSession>, IQuizSessionRepository
    {
        public QuizSessionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<QuizSession> GetBySessionCodeAsync(string sessionCode)
        {
            return await _dbContext.QuizSessions
                .Include(s => s.Quiz)
                .FirstOrDefaultAsync(s => s.SessionCode == sessionCode);
        }

        public async Task<QuizSession> GetByIdWithDetailsAsync(int id)
        {
            return await _dbContext.QuizSessions
                .Include(s => s.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(q => q.Options)
                .Include(s => s.Participants)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<QuizSession>> GetActiveSessionsByHostIdAsync(string hostId)
        {
            return await _dbContext.QuizSessions
                .Include(s => s.Quiz)
                .Where(s => s.HostId == hostId && (s.Status == SessionStatus.Waiting || s.Status == SessionStatus.InProgress))
                .ToListAsync();
        }

        public async Task<SessionParticipant> GetParticipantAsync(int sessionId, string userId)
        {
            return await _dbContext.SessionParticipants
                .FirstOrDefaultAsync(p => p.SessionId == sessionId && p.UserId == userId);
        }

        public async Task<IEnumerable<SessionParticipant>> GetParticipantsAsync(int sessionId)
        {
            return await _dbContext.SessionParticipants
                .Include(p => p.User)
                .Where(p => p.SessionId == sessionId)
                .ToListAsync();
        }

        public async Task AddParticipantAsync(SessionParticipant participant)
        {
            await _dbContext.SessionParticipants.AddAsync(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateParticipantAsync(SessionParticipant participant)
        {
            _dbContext.SessionParticipants.Update(participant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveParticipantAsync(SessionParticipant participant)
        {
            _dbContext.SessionParticipants.Remove(participant);
            await _dbContext.SaveChangesAsync();
        }
    }
}
