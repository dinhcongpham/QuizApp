using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Interfaces.IRepositories;
using QuizApp.QuizApp.Shared.DTOs;
using System.Security.Claims;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetMyQuizzes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quizzes = await _quizService.GetQuizzesByUserIdAsync(userId);
            return Ok(quizzes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuiz(int id)
        {
            var quiz = await _quizService.GetQuizByIdAsync(id);
            return Ok(quiz);
        }

        [HttpGet("code/{accessCode}")]
        public async Task<ActionResult<QuizDto>> GetQuizByCode(string accessCode)
        {
            var quiz = await _quizService.GetQuizByAccessCodeAsync(accessCode);
            return Ok(quiz);
        }

        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetPublicQuizzes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var quizzes = await _quizService.GetPublicQuizzesAsync(page, pageSize);
            return Ok(quizzes);
        }

        [HttpPost]
        public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] CreateQuizDto createQuizDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quiz = await _quizService.CreateQuizAsync(createQuizDto, userId);
            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
        }
    }
}
