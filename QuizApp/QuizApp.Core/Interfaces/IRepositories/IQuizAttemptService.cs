using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces.IRepositories
{
    public interface IQuizAttemptService
    {
        Task<AttemptDto> StartAttemptAsync(StartAttemptDto startAttemptDto, string userId);
        Task<AttemptDto> SubmitAnswerAsync(SubmitAnswerDto submitAnswerDto, string userId);
        Task<AttemptDto> CompleteAttemptAsync(int attemptId, string userId);
        Task<AttemptDto> GetAttemptByIdAsync(int attemptId);
        Task<IEnumerable<AttemptDto>> GetAttemptsByUserIdAsync(string userId);
        Task<IEnumerable<AttemptDto>> GetAttemptsByQuizIdAsync(int quizId);
        Task<AttemptResultDto> GetAttemptResultAsync(int attemptId, string userId);
    }
}
