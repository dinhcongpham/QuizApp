using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        // GET: api/quiz
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quizzes = await _quizService.GetAllAsync();
            return Ok(quizzes);
        }

        // GET: api/quiz/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _quizService.GetByIdAsync(id);
            if (quiz == null) return NotFound(new { message = "Quiz not found." });

            return Ok(quiz);
        }

        // POST: api/quiz
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdQuiz = await _quizService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdQuiz.QuizId }, createdQuiz);
        }

        // PUT: api/quiz/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateQuizDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _quizService.UpdateAsync(id, dto);
            if (!updated) return NotFound(new { message = "Quiz not found." });

            return NoContent(); // 204
        }

        // DELETE: api/quiz/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _quizService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "Quiz not found." });

            return NoContent(); // 204
        }
    }
}
