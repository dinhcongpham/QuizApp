using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.API.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMemoryCache _cache;
        private readonly IGameRoomService _gameRoomService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IMemoryCache cache, IGameRoomService gameRoomService, ILogger<GameHub> logger)
        {
            _cache = cache;
            _gameRoomService = gameRoomService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateRoom(int quizId, int userId)
        {
            try
            {
                var room = await _gameRoomService.CreateRoomAsync(quizId, userId);
                await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode);
                await Clients.Caller.SendAsync("RoomCreated", room);
                _logger.LogInformation($"Room created: {room.RoomCode} by user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating room for quiz {quizId} by user {userId}");
                await Clients.Caller.SendAsync("Error", "Failed to create room");
            }
        }

        public async Task JoinRoom(string roomCode, int userId)
        {
            try
            {
                var room = await _gameRoomService.JoinRoomAsync(roomCode, userId);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
                await Clients.Group(roomCode).SendAsync("UserJoined", room);
                _logger.LogInformation($"User {userId} joined room {roomCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error joining room {roomCode} by user {userId}");
                await Clients.Caller.SendAsync("Error", "Failed to join room");
            }
        }

        public async Task StartGame(string roomCode)
        {
            try
            {
                var gameState = await _gameRoomService.StartGameAsync(roomCode);
                await Clients.Group(roomCode).SendAsync("GameStarted", gameState);
                _logger.LogInformation($"Game started in room {roomCode}");

                // Start timer for first question
                await StartQuestionTimer(roomCode, gameState.CurrentQuestionIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting game in room {roomCode}");
                await Clients.Caller.SendAsync("Error", "Failed to start game");
            }
        }

        public async Task SubmitAnswer(string roomCode, int userId, int questionId, string answer, decimal timeTaken)
        {
            try
            {
                var result = await _gameRoomService.SubmitAnswerAsync(roomCode, userId, questionId, answer, timeTaken);
                await Clients.Group(roomCode).SendAsync("AnswerSubmitted", result);
                _logger.LogInformation($"Answer submitted by user {userId} in room {roomCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error submitting answer in room {roomCode} by user {userId}");
                await Clients.Caller.SendAsync("Error", "Failed to submit answer");
            }
        }

        private async Task StartQuestionTimer(string roomCode, int questionIndex)
        {
            // Wait for 20 seconds
            await Task.Delay(20000);

            try
            {
                var leaderboard = await _gameRoomService.GetLeaderboardAsync(roomCode, questionIndex);
                await Clients.Group(roomCode).SendAsync("QuestionEnded", leaderboard);
                _logger.LogInformation($"Question {questionIndex} ended in room {roomCode}");

                // Check if game should continue
                var gameState = _cache.Get<GameState>($"gamestate_{roomCode}");
                if (gameState != null && gameState.CurrentQuestionIndex < gameState.TotalQuestions - 1)
                {
                    gameState.CurrentQuestionIndex++;
                    _cache.Set($"gamestate_{roomCode}", gameState);
                    await Clients.Group(roomCode).SendAsync("NextQuestion", gameState);
                    await StartQuestionTimer(roomCode, gameState.CurrentQuestionIndex);
                }
                else
                {
                    var finalResults = await _gameRoomService.GetFinalResultsAsync(roomCode);
                    await Clients.Group(roomCode).SendAsync("GameEnded", finalResults);
                    _logger.LogInformation($"Game ended in room {roomCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in question timer for room {roomCode}");
                await Clients.Group(roomCode).SendAsync("Error", "Game error occurred");
            }
        }
    }
} 