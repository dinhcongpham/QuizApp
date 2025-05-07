using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
            try
            {
                // Get all rooms the user is in
                var userRooms = await _gameRoomService.GetUserRoomsAsync(Context.ConnectionId);
                
                foreach (var roomCode in userRooms)
                {
                    // Remove user from room
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
                    
                    // Check if room is empty and cleanup if needed
                    var room = await _gameRoomService.GetRoomAsync(roomCode);
                    if (room != null && room.Participants.Count == 0)
                    {
                        await _gameRoomService.CleanupRoomAsync(roomCode);
                        _logger.LogInformation($"Room {roomCode} cleaned up after all users disconnected");
                    }
                    else
                    {
                        await Clients.Group(roomCode).SendAsync("UserLeft", Context.ConnectionId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling disconnection for client {Context.ConnectionId}");
            }
            finally
            {
                await base.OnDisconnectedAsync(exception);
            }
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
                var room = await _gameRoomService.GetRoomAsync(roomCode);
                if (room.Status == "InProgress")
                {
                    var (roomInfo, gameState, leaderboard) = await _gameRoomService.JoinRoomWhenProgressAsync(roomCode, userId);
                    await Clients.Group(roomCode).SendAsync("NotifyUserJoined", roomInfo, leaderboard);

                    await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
                    await Clients.Client(Context.ConnectionId).SendAsync("UserJoinedWhenGameProgress",
                        roomInfo,
                        gameState,
                        leaderboard
                    );
                    _logger.LogInformation($"User {userId} joined room {roomCode} in Progress state");
                }

                if (room.Status == "Waiting")
                {
                    var roomInfo = await _gameRoomService.JoinRoomAsync(roomCode, userId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
                    await Clients.Group(roomCode).SendAsync("UserJoined", roomInfo);
                    _logger.LogInformation($"User {userId} joined room {roomCode} in Wating state");
                }
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
                await _gameRoomService.SubmitAnswerAsync(roomCode, userId, questionId, answer, timeTaken);
                _logger.LogInformation($"Answer submitted by user {userId} in room {roomCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error submitting answer in room {roomCode} by user {userId}");
                await Clients.Caller.SendAsync("Error", "Failed to submit answer");
            }
        }

        //public async Task EndQuestionTimer(string roomCode, int questionIndex)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Host ended timer for question {questionIndex} in room {roomCode}");

        //        // Get and send current leaderboard
        //        var leaderboard = await _gameRoomService.GetLeaderboardAsync(roomCode, questionIndex);
        //        await Clients.Group(roomCode).SendAsync("QuestionEnded", leaderboard);

        //        // Check and update game state
        //        var gameState = await _gameRoomService.GetGameStateAsync(roomCode);
        //        if (gameState != null && gameState.CurrentQuestionIndex < gameState.TotalQuestions - 1)
        //        {
        //            gameState.CurrentQuestionIndex++;
        //            await _gameRoomService.UpdateGameStateAsync(roomCode, gameState);
        //            await Clients.Group(roomCode).SendAsync("NextQuestion", gameState);
        //        }
        //        else
        //        {
        //            var finalResults = await _gameRoomService.GetGameStateAsync(roomCode);
        //            await _gameRoomService.SaveRoomDataToDatabaseAsync(roomCode);
        //            await _gameRoomService.CleanupRoomAsync(roomCode);
        //            await Clients.Group(roomCode).SendAsync("GameEnded", finalResults);
        //            _logger.LogInformation($"Game ended in room {roomCode}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error handling question end in room {roomCode}");
        //        await Clients.Group(roomCode).SendAsync("Error", "Game error occurred");
        //    }
        //}
    }
} 