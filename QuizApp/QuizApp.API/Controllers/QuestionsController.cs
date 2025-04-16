using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // POST: api/questions
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _questionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.QuestionId }, created);
        }

        // GET: api/questions/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        // GET: api/questions/quiz/{quizId}
        [HttpGet("quiz/{quizId}")]
        [Authorize]
        public async Task<IActionResult> GetByQuizId(int quizId)
        {
            var questions = await _questionService.GetByQuizIdAsync(quizId);
            return Ok(questions);
        }

        // PUT: api/questions/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _questionService.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        // DELETE: api/questions/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _questionService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
