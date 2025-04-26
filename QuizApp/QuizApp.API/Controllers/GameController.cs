using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameRoomService _gameRoomService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameRoomService gameRoomService, ILogger<GameController> logger)
        {
            _gameRoomService = gameRoomService;
            _logger = logger;
        }

        // GET: api/game/rooms
        [HttpGet("rooms")]
        [Authorize]
        public async Task<ActionResult<List<GameRoomDto>>> GetAllRooms()
        {
            try
            {
                _logger.LogInformation("Getting all active rooms");
                var rooms = await _gameRoomService.GetAllActiveRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rooms");
                return StatusCode(500, "An error occurred while getting rooms");
            }
        }

        // GET: api/game/rooms/{roomCode}
        [HttpGet("rooms/{roomCode}")]
        [Authorize]
        public async Task<ActionResult<GameRoomDto>> GetRoom(string roomCode)
        {
            try
            {
                _logger.LogInformation("Getting room {RoomCode}", roomCode);
                var room = await _gameRoomService.GetRoomAsync(roomCode);
                if (room == null)
                {
                    _logger.LogWarning("Room {RoomCode} not found", roomCode);
                    return NotFound();
                }
                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room {RoomCode}", roomCode);
                return StatusCode(500, "An error occurred while getting room");
            }
        }

        // POST: api/game/rooms
        [HttpPost("rooms")]
        [Authorize]
        public async Task<ActionResult<GameRoomDto>> CreateRoom([FromBody] CreateRoomDto dto)
        {
            try
            {
                _logger.LogInformation("Creating room for quiz {QuizId} by user {UserId}", dto.QuizId, dto.UserId);
                var room = await _gameRoomService.CreateRoomAsync(dto.QuizId, dto.UserId);
                return CreatedAtAction(nameof(GetRoom), new { roomCode = room.RoomCode }, room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room for quiz {QuizId} by user {UserId}", dto.QuizId, dto.UserId);
                return StatusCode(500, "An error occurred while creating room");
            }
        }

        // DELETE: api/game/rooms/{roomCode}
        [HttpDelete("rooms/{roomCode}")]
        [Authorize]
        public async Task<IActionResult> DeleteRoom(string roomCode)
        {
            try
            {
                _logger.LogInformation("Deleting room {RoomCode}", roomCode);
                await _gameRoomService.CleanupRoomAsync(roomCode);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room {RoomCode}", roomCode);
                return StatusCode(500, "An error occurred while deleting room");
            }
        }

        // GET: api/game/rooms/{roomCode}/results
        [HttpGet("rooms/{roomCode}/results")]
        [Authorize]
        public async Task<ActionResult<GameResultsDto>> GetRoomResults(string roomCode)
        {
            try
            {
                _logger.LogInformation("Getting results for room {RoomCode}", roomCode);
                var results = await _gameRoomService.GetFinalResultsAsync(roomCode);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting results for room {RoomCode}", roomCode);
                return StatusCode(500, "An error occurred while getting results");
            }
        }
    }
} 