using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        // GET: api/quiz
        [HttpGet("users/{userId}")]
        [Authorize]
        public async Task<ActionResult<List<QuizResponseDto>>> GetAll(int userId)
        {
            try
            {
                _logger.LogInformation("Attempting to get all quizzes for user {UserId}", userId);
                var quizzes = await _quizService.GetAllAsync(userId);
                if (quizzes == null || quizzes.Count == 0)
                {
                    _logger.LogWarning("No quizzes found for user {UserId}", userId);
                    return NotFound(new { message = "No quizzes found for this user." });
                }
                _logger.LogInformation("Successfully retrieved {Count} quizzes for user {UserId}", quizzes.Count, userId);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all quizzes for user {UserId}", userId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // GET: api/quiz/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to get quiz with ID: {QuizId}", id);
                var quiz = await _quizService.GetByIdAsync(id);
                if (quiz == null)
                {
                    _logger.LogWarning("Quiz not found with ID: {QuizId}", id);
                    return NotFound(new { message = "Quiz not found." });
                }
                _logger.LogInformation("Successfully retrieved quiz with ID: {QuizId}", id);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting quiz with ID: {QuizId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // POST: api/quiz
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting to create new quiz");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var createdQuiz = await _quizService.CreateAsync(dto);
                _logger.LogInformation("Successfully created quiz with ID: {QuizId}", createdQuiz.QuizId);
                return CreatedAtAction(nameof(GetById), new { id = createdQuiz.QuizId }, createdQuiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating quiz");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // PUT: api/quiz/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateQuizDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting to update quiz with ID: {QuizId}", id);
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var updated = await _quizService.UpdateAsync(id, dto);
                if (!updated)
                {
                    _logger.LogWarning("Quiz not found with ID: {QuizId}", id);
                    return NotFound(new { message = "Quiz not found." });
                }
                _logger.LogInformation("Successfully updated quiz with ID: {QuizId}", id);
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating quiz with ID: {QuizId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // DELETE: api/quiz/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete quiz with ID: {QuizId}", id);
                var deleted = await _quizService.DeleteAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Quiz not found with ID: {QuizId}", id);
                    return NotFound(new { message = "Quiz not found." });
                }
                _logger.LogInformation("Successfully deleted quiz with ID: {QuizId}", id);
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting quiz with ID: {QuizId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
