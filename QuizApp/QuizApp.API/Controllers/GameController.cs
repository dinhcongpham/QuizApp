using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GameController : ControllerBase
    {
        private readonly IGameRoomService _gameRoomService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameRoomService gameRoomService, ILogger<GameController> logger)
        {
            _gameRoomService = gameRoomService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
        {
            try
            {
                var room = await _gameRoomService.CreateRoomAsync(dto.QuizId, dto.UserId);
                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomDto dto)
        {
            try
            {
                var room = await _gameRoomService.JoinRoomAsync(dto.RoomCode, dto.UserId);
                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining room");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameDto dto)
        {
            try
            {
                var gameState = await _gameRoomService.StartGameAsync(dto.RoomCode);
                return Ok(gameState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("submit-answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerDto dto)
        {
            try
            {
                var result = await _gameRoomService.SubmitAnswerAsync(
                    dto.RoomCode,
                    dto.UserId,
                    dto.QuestionId,
                    dto.Answer,
                    dto.TimeTaken
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting answer");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("leaderboard/{roomCode}/{questionId}")]
        public async Task<IActionResult> GetLeaderboard(string roomCode, int questionId)
        {
            try
            {
                var leaderboard = await _gameRoomService.GetLeaderboardAsync(roomCode, questionId);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leaderboard");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("results/{roomCode}")]
        public async Task<IActionResult> GetFinalResults(string roomCode)
        {
            try
            {
                var results = await _gameRoomService.GetFinalResultsAsync(roomCode);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting final results");
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateRoomDto
    {
        public int QuizId { get; set; }
        public int UserId { get; set; }
    }

    public class JoinRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int UserId { get; set; }
    }

    public class StartGameDto
    {
        public string RoomCode { get; set; } = string.Empty;
    }

    public class SubmitAnswerDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; } = string.Empty;
        public decimal TimeTaken { get; set; }
    }
} 