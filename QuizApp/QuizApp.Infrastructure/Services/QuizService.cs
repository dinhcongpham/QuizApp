using Microsoft.EntityFrameworkCore;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Infrastructure.Data;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Infrastructure.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;

        public QuizService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuizResponseDto>> GetAllAsync()
        {
            return await _context.Quizs
                .Select(q => new QuizResponseDto
                {
                    QuizId = q.Id,
                    Title = q.Title,
                    Description = q.Description
                })
                .ToListAsync();
        }

        public async Task<QuizResponseDto?> GetByIdAsync(int id)
        {
            var quiz = await _context.Quizs.FindAsync(id);
            if (quiz == null) return null;

            return new QuizResponseDto
            {
                QuizId = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description
            };
        }

        public async Task<QuizResponseDto> CreateAsync(CreateQuizDto quizDto)
        {
            var quiz = new Quiz
            {
                Title = quizDto.Title,
                Description = quizDto.Description,
                OwnerId = quizDto.OwnerId
            };

            _context.Quizs.Add(quiz);
            await _context.SaveChangesAsync();

            return new QuizResponseDto
            {
                QuizId = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateQuizDto dto)
        {
            var quiz = await _context.Quizs.FindAsync(id);
            if (quiz == null) return false;

            quiz.Title = dto.Title;
            quiz.Description = dto.Description;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await _context.Quizs.FindAsync(id);
            if (quiz == null) return false;

            _context.Quizs.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
