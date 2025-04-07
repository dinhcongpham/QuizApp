using QuizApp.QuizApp.Core.Entities;

namespace QuizApp.QuizApp.Core.Interfaces.IRepositories
{
    public interface IQuizSessionRepository
    {
        public interface IQuizSessionRepository : IGenericRepository<QuizSession>
        {
            Task<QuizSession> GetBySessionCodeAsync(string sessionCode);
            Task<QuizSession> GetByIdWithDetailsAsync(int id);
            Task<IEnumerable<QuizSession>> GetActiveSessionsByHostIdAsync(string hostId);
            Task<SessionParticipant> GetParticipantAsync(int sessionId, string userId);
            Task<IEnumerable<SessionParticipant>> GetParticipantsAsync(int sessionId);
            Task AddParticipantAsync(SessionParticipant participant);
            Task UpdateParticipantAsync(SessionParticipant participant);
            Task RemoveParticipantAsync(SessionParticipant participant);
        }
    }
}
