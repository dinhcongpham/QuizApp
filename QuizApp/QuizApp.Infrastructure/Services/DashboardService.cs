using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Infrastructure.Data;
using QuizApp.QuizApp.Shared.DTOs;
using System.Threading.Tasks;

namespace QuizApp.QuizApp.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserStatsDTO> GetUserStatsAsync(int userId)
        {
            var stats = new UserStatsDTO();

            // Get total quizzes created by user
            stats.TotalQuizzesCreated = await _context.Quizs
                .CountAsync(q => q.OwnerId == userId);

            // Get total quizzes participated
            stats.TotalQuizzesParticipated = await _context.RoomParticipants
                .CountAsync(rp => rp.UserId == userId);

            // Calculate accuracy rate
            var userAnswers = await _context.UserAnswers
                .Where(ua => ua.UserId == userId)
                .ToListAsync();

            int totalAnswers = userAnswers.Count;
            int correctAnswers = userAnswers.Count(ua => ua.IsCorrect);

            stats.AccuracyRate = totalAnswers > 0
                ? Math.Round((double)correctAnswers / totalAnswers * 100, 2)
                : 0;

            return stats;
        }

        public async Task<QuizStatsDTO> GetQuizsStatsAsync(int userId)
        {
            var raw = await (from ua in _context.UserAnswers
                             join qs in _context.Questions on ua.QuestionId equals qs.Id
                             join r in _context.Rooms on ua.RoomId equals r.Id
                             join q in _context.Quizs on r.QuizId equals q.Id
                             where ua.UserId == userId
                             orderby r.StartedAt descending, r.Id, qs.Id
                             select new QuizRoomRawEntry
                             {
                                 RoomId = r.Id,
                                 StartedAt = r.StartedAt,
                                 QuizTitle = q.Title,
                                 QuizDescription = q.Description,

                                 Content = qs.Content,
                                 OptionA = qs.OptionA,
                                 OptionB = qs.OptionB,
                                 OptionC = qs.OptionC,
                                 OptionD = qs.OptionD,
                                 CorrectOption = qs.CorrectOption,
                                 SelectedOption = ua.SelectedOption
                             }).ToListAsync();

            var grouped = raw
                .GroupBy(x => new { x.RoomId, x.StartedAt, x.QuizTitle, x.QuizDescription })
                .Select(g => new DetailQuizStatsDTO
                {
                    RoomId = g.Key.RoomId,
                    StartedAt = g.Key.StartedAt,
                    Title = g.Key.QuizTitle,
                    Description = g.Key.QuizDescription,
                    UserAnswers = g.Select(x => new DetailUserAnswersDTO
                    {
                        Content = x.Content,
                        OptionA = x.OptionA,
                        OptionB = x.OptionB,
                        OptionC = x.OptionC,
                        OptionD = x.OptionD,
                        CorrectOption = x.CorrectOption,
                        SelectedOption = x.SelectedOption
                    }).ToList()
                }).ToList();

            return new QuizStatsDTO
            {
                Quiz = grouped
            };
        }

    }
} 