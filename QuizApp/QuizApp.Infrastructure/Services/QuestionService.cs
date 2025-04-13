using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Infrastructure.Data;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly ApplicationDbContext _context;

        public QuestionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuestionResponseDto> CreateAsync(CreateQuestionDto dto)
        {
            var question = new Question
            {
                QuizId = dto.QuizId,
                Content = dto.Content,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                CorrectOption = dto.CorrectOption,
                CreatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return ToDto(question);
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetByQuizIdAsync(int quizId)
        {
            return await _context.Questions
                .Where(q => q.QuizId == quizId)
                .Select(q => ToDto(q))
                .ToListAsync();
        }

        public async Task<QuestionResponseDto?> GetByIdAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            return question == null ? null : ToDto(question);
        }

        public async Task<bool> UpdateAsync(int id, UpdateQuestionDto dto)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return false;

            question.Content = dto.Content;
            question.OptionA = dto.OptionA;
            question.OptionB = dto.OptionB;
            question.OptionC = dto.OptionC;
            question.OptionD = dto.OptionD;
            question.CorrectOption = dto.CorrectOption;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return false;

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        private static QuestionResponseDto ToDto(Question q) => new()
        {
            QuestionId = q.Id,
            QuizId = q.QuizId,
            Content = q.Content,
            OptionA = q.OptionA,
            OptionB = q.OptionB,
            OptionC = q.OptionC,
            OptionD = q.OptionD,
            CorrectOption = q.CorrectOption,
            CreatedAt = q.CreatedAt
        };
    }
}
