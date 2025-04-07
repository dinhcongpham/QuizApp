using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces.IRepositories
{
    public interface IQuizSessionService
    {
        Task<SessionDto> CreateSessionAsync(CreateSessionDto createSessionDto, string userId);
        Task<SessionDto> StartSessionAsync(int sessionId, string userId);
        Task<SessionDto> EndSessionAsync(int sessionId, string userId);
        Task<SessionDto> GetSessionByIdAsync(int sessionId);
        Task<SessionDto> GetSessionByCodeAsync(string sessionCode);
        Task<IEnumerable<SessionDto>> GetActiveSessionsByHostIdAsync(string hostId);
        Task<ParticipantDto> JoinSessionAsync(JoinSessionDto joinSessionDto, string userId);
        Task<ParticipantDto> LeaveSessionAsync(int sessionId, string userId);
        Task<IEnumerable<ParticipantDto>> GetSessionParticipantsAsync(int sessionId);
        Task<QuestionDto> GetCurrentQuestionAsync(int sessionId);
        Task<QuestionDto> MoveToNextQuestionAsync(int sessionId, string userId);
    }
}
