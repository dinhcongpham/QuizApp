using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IDashboardService
    {
        Task<UserStatsDTO> GetUserStatsAsync(int userId);
        Task<QuizStatsDTO> GetQuizsStatsAsync(int userId);
    }
} 