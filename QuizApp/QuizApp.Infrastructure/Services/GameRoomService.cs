using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Infrastructure.Data;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Services
{
    public class GameRoomService : IGameRoomService
    {
        private readonly IMemoryCache _cache;
        private readonly IQuizService _quizService;
        private readonly IQuestionService _questionService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<GameRoomService> _logger;
        private const string ROOM_PREFIX = "ROOM_";
        private const string GAME_STATE_PREFIX = "GAME_";
        private const string USER_ROOMS_PREFIX = "USER_";

        public GameRoomService(
            IMemoryCache cache, 
            IQuizService quizService, 
            IQuestionService questionService, 
            IUserRepository userRepository, 
            ILogger<GameRoomService> logger,
            ApplicationDbContext dbContext
        )
        {
            _cache = cache;
            _quizService = quizService;
            _questionService = questionService;
            _userRepository = userRepository;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<GameRoomDto> CreateRoomAsync(int quizId, int userId)
        {
            try
            {
                _logger.LogInformation("Creating room for quiz {QuizId} by user {UserId}", quizId, userId);
                
                // Check if user already has a room
                var existingRoom = await GetRoomAsync(userId.ToString());
                if (existingRoom != null)
                {
                    throw new InvalidOperationException($"User {userId} already has a room: {existingRoom.RoomCode}");
                }

                // Get quiz details
                var quiz = await _quizService.GetByIdAsync(quizId);
                if (quiz == null)
                {
                    throw new ArgumentException($"Quiz with ID {quizId} not found");
                }
                
                var roomCode = GenerateRoomCode();

                // Create room with user ID as room code
                var room = new GameRoomDto
                {
                    RoomCode = roomCode,
                    QuizId = quizId,
                    HostUserId = userId,
                    Questions = (await _questionService.GetByQuizIdAsync(quizId)).Select(q => new QuestionResponseDto
                    {
                        QuestionId = q.QuestionId,
                        QuizId = q.QuizId,
                        Content = q.Content,
                        OptionA = q.OptionA,
                        OptionB = q.OptionB,
                        OptionC = q.OptionC,
                        OptionD = q.OptionD,
                        CorrectOption = q.CorrectOption,
                        CreatedAt = q.CreatedAt
                    }).ToList(),
                    Participants = new []
                    {
                        new RoomParticipantDto
                        {
                            UserId = userId,
                            UserName = (await _userRepository.GetByIdAsync(userId))?.FullName ?? "Host",
                            JoinedAt = DateTime.UtcNow
                        }
                    }.ToList(),
                    Status = "WAITING", 
                    CreatedAt = DateTime.UtcNow,
                    StartedAt = DateTime.MinValue
                };

                // Store room in cache
                _cache.Set($"{ROOM_PREFIX}{roomCode}", room);

                // Add to active rooms list
                var activeRooms = _cache.Get<List<string>>("ACTIVE_ROOMS") ?? new List<string>();
                activeRooms.Add(roomCode);
                _cache.Set("ACTIVE_ROOMS", activeRooms, TimeSpan.FromHours(1));

                _logger.LogInformation("Room {RoomCode} created successfully", userId);
                return room;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room for quiz {QuizId} by user {UserId}", quizId, userId);
                throw;
            }
        }

        public async Task<GameRoomDto> JoinRoomAsync(string roomCode, int userId)
        {
            try
            {
                _logger.LogInformation("User {UserId} attempting to join room {RoomCode}", userId, roomCode);
                
                var room = await GetRoomAsync(roomCode);
                if (room == null)
                {
                    throw new ArgumentException($"Room {roomCode} not found");
                }

                if (room.Status != "WAITING")
                {
                    throw new InvalidOperationException("Cannot join a room that has already started");
                }

                // Add user to participants if not already there
                if (!room.Participants.Any(p => p.UserId == userId))
                {
                    room.Participants.Add(new RoomParticipantDto
                    {
                        UserId = userId,
                        JoinedAt = DateTime.UtcNow
                    });

                    // Update room in cache
                    _cache.Set($"{ROOM_PREFIX}{roomCode}", room);
                }

                _logger.LogInformation("User {UserId} joined room {RoomCode} successfully", userId, roomCode);
                return room;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining room {RoomCode} by user {UserId}", roomCode, userId);
                throw;
            }
        }

        public async Task<GameStateDto> StartGameAsync(string roomCode)
        {
            try
            {
                _logger.LogInformation("Starting game in room {RoomCode}", roomCode);
                
                var room = await GetRoomAsync(roomCode);
                if (room == null)
                {
                    throw new ArgumentException($"Room {roomCode} not found");
                }

                if (room.Status != "WAITING")
                {
                    throw new InvalidOperationException("Game has already started");
                }

                // Create game state
                var gameState = new GameStateDto
                {
                    RoomCode = roomCode,
                    CurrentQuestionIndex = 0,
                    TotalQuestions = room.Questions.Count,
                    StartTime = DateTime.UtcNow,
                    Status = "IN_PROGRESS"
                };

                // Update room status
                room.Status = "IN_PROGRESS";
                room.StartedAt = DateTime.UtcNow;
                _cache.Set($"{ROOM_PREFIX}{roomCode}", room);

                // Store game state
                _cache.Set($"{GAME_STATE_PREFIX}{roomCode}", gameState);

                _logger.LogInformation("Game started in room {RoomCode}", roomCode);
                return gameState;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game in room {RoomCode}", roomCode);
                throw;
            }
        }

        public async Task SubmitAnswerAsync(string roomCode, int userId, int questionId, string answer, decimal timeTaken)
        {
            try
            {
                _logger.LogInformation("User {UserId} submitting answer for question {QuestionId} in room {RoomCode}", 
                    userId, questionId, roomCode);

                var gameState = await GetGameStateAsync(roomCode);
                if (gameState == null)
                {
                    throw new ArgumentException($"Game not found for room {roomCode}");
                }

                var room = await GetRoomAsync(roomCode);
                if (room == null)
                {
                    throw new ArgumentException($"Room {roomCode} not found");
                }

                var question = room.Questions.FirstOrDefault(q => q.QuestionId == questionId);
                if (question == null)
                {
                    throw new ArgumentException($"Question {questionId} not found in room {roomCode}");
                }

                // Calculate score based on correctness and time taken
                var isCorrect = answer == question.CorrectOption;
                var score = isCorrect ? CalculateScore(timeTaken) : 0;

                // Update leaderboard
                await UpdateLeaderboardAsync(roomCode, questionId, userId, score, timeTaken);

                _logger.LogInformation("Answer submitted successfully by user {UserId} in room {RoomCode}", 
                    userId, roomCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting answer in room {RoomCode} by user {UserId}", 
                    roomCode, userId);
                throw;
            }
        }

        public async Task<LeaderboardSnapshotDto> GetLeaderboardAsync(string roomCode, int questionId)
        {
            try
            {
                _logger.LogInformation("Getting leaderboard for question {QuestionId} in room {RoomCode}", 
                    questionId, roomCode);

                var leaderboard = await Task.FromResult(_cache.Get<LeaderboardSnapshotDto>($"LEADERBOARD_{roomCode}_{questionId}"));
                if (leaderboard == null)
                {
                    throw new ArgumentException($"Leaderboard not found for question {questionId} in room {roomCode}");
                }

                return leaderboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leaderboard for question {QuestionId} in room {RoomCode}", 
                    questionId, roomCode);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRoomsAsync(string connectionId)
        {
            try
            {
                _logger.LogInformation("Getting rooms for user {ConnectionId}", connectionId);
                var rooms = await Task.FromResult(_cache.Get<List<string>>($"{USER_ROOMS_PREFIX}{connectionId}") ?? new List<string>());
                return rooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rooms for user {ConnectionId}", connectionId);
                throw;
            }
        }

        public async Task<GameStateDto?> GetGameStateAsync(string roomCode)
        {
            try
            {
                _logger.LogInformation("Getting game state for room {RoomCode}", roomCode);
                var gameState = await Task.FromResult(_cache.Get<GameStateDto>($"{GAME_STATE_PREFIX}{roomCode}"));
                return gameState;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game state for room {RoomCode}", roomCode);
                throw;
            }
        }

        public async Task UpdateGameStateAsync(string roomCode, GameStateDto gameState)
        {
            try
            {
                _logger.LogInformation("Updating game state for room {RoomCode}", roomCode);
                await Task.Run(() => _cache.Set($"{GAME_STATE_PREFIX}{roomCode}", gameState));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating game state for room {RoomCode}", roomCode);
                throw;
            }
        }


        public async Task CleanupRoomAsync(string roomCode)
        {
            try
            {
                _logger.LogInformation("Cleaning up room {RoomCode}", roomCode);
                
                // Remove room data
                await Task.Run(() => _cache.Remove($"{ROOM_PREFIX}{roomCode}"));
                
                // Remove game state
                await Task.Run(() => _cache.Remove($"{GAME_STATE_PREFIX}{roomCode}"));
                
                // Remove user answers
                await Task.Run(() => _cache.Remove($"USER_ANSWERS_{roomCode}"));
                
                // Remove leaderboard
                await Task.Run(() => _cache.Remove($"LEADERBOARD_{roomCode}"));

                // Remove from active rooms list
                var activeRooms = await Task.FromResult(_cache.Get<List<string>>("ACTIVE_ROOMS") ?? new List<string>());
                activeRooms.Remove(roomCode);
                await Task.Run(() => _cache.Set("ACTIVE_ROOMS", activeRooms, TimeSpan.FromHours(1)));

                _logger.LogInformation("Room {RoomCode} cleaned up successfully", roomCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up room {RoomCode}", roomCode);
                throw;
            }
        }

        public async Task<GameRoomDto?> GetRoomAsync(string roomCode)
        {
            try
            {
                _logger.LogInformation("Getting room {RoomCode}", roomCode);
                var room = await Task.FromResult(_cache.Get<GameRoomDto>($"{ROOM_PREFIX}{roomCode}"));
                return room;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room {RoomCode}", roomCode);
                throw;
            }
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private int CalculateScore(decimal timeTaken)
        {
            // Base score is 10000, reduced by time taken
            const decimal maxTime = 20000;
            const decimal baseScore = 10000;
            
            if (timeTaken >= maxTime) 
                return 0;

            var score = baseScore * (1 - (timeTaken / maxTime));
            return (int)Math.Round(score);
        }

        private async Task UpdateLeaderboardAsync(string roomCode, int questionId, int userId, int score, decimal timeTaken)
        {
            // Get or create user answers
            var userAnswers = await Task.FromResult(_cache.Get<UserAnswersDto>($"USER_ANSWERS_{roomCode}"))
                ?? new UserAnswersDto
                {
                    RoomId = int.Parse(roomCode),
                    UserAnswers = new List<UserAnswerEntryDto>()
                };

            // Add or update user's answer
            var existingAnswer = userAnswers.UserAnswers.FirstOrDefault(a => a.UserId == userId && a.QuestionId == questionId);
            if (existingAnswer != null)
            {
                existingAnswer.Score = score;
                existingAnswer.TimeTaken = timeTaken;
                existingAnswer.IsCorrect = score > 0;
            }
            else
            {
                userAnswers.UserAnswers.Add(new UserAnswerEntryDto
                {
                    UserId = userId,
                    QuestionId = questionId,
                    Score = score,
                    TimeTaken = timeTaken,
                    IsCorrect = score > 0
                });
            }

            // Save user answers
            _cache.Set($"USER_ANSWERS_{roomCode}", userAnswers);

            // Get or create leaderboard
            var leaderboard = await Task.FromResult(_cache.Get<LeaderboardSnapshotDto>($"LEADERBOARD_{roomCode}"))
                ?? new LeaderboardSnapshotDto
                {
                    RoomId = int.Parse(roomCode),
                    Entries = new List<LeaderboardSnapshotEntryDto>()
                };

            // Update or add user's score in leaderboard
            var existingEntry = leaderboard.Entries.FirstOrDefault(e => e.UserId == userId);
            if (existingEntry != null)
            {
                existingEntry.Score += (int)score;
            }
            else
            {
                leaderboard.Entries.Add(new LeaderboardSnapshotEntryDto
                {
                    UserId = userId,
                    Score = (int)score
                });
            }

            // Sort entries by score (descending)
            leaderboard.Entries = leaderboard.Entries
                .OrderByDescending(e => e.Score)
                .ToList();

            // Save leaderboard
            _cache.Set($"LEADERBOARD_{roomCode}", leaderboard);
        }

        public async Task SaveRoomDataToDatabaseAsync(string roomCode)
        {
            try
            {
                _logger.LogInformation("Saving room {RoomCode} data to database", roomCode);

                // Get room data from cache
                var room = await GetRoomAsync(roomCode);
                if (room == null)
                {
                    throw new ArgumentException($"Room {roomCode} not found");
                }

                // Get user answers from cache
                var userAnswers = await Task.FromResult(_cache.Get<UserAnswersDto>($"USER_ANSWERS_{roomCode}"));
                if (userAnswers == null)
                {
                    throw new ArgumentException($"User answers for room {roomCode} not found");
                }

                // Get leaderboard from cache
                var leaderboard = await Task.FromResult(_cache.Get<LeaderboardSnapshotDto>($"LEADERBOARD_{roomCode}"));
                if (leaderboard == null)
                {
                    throw new ArgumentException($"Leaderboard for room {roomCode} not found");
                }

                // Save room data
                var roomEntity = new Room
                {
                    RoomCode = room.RoomCode,
                    QuizId = room.QuizId,
                    HostUserId = room.HostUserId,
                    Status = room.Status,
                    CreatedAt = room.CreatedAt,
                    StartedAt = room.StartedAt
                };
                await _dbContext.Rooms.AddAsync(roomEntity);
                await _dbContext.SaveChangesAsync();

                // Save room participants
                foreach (var participant in room.Participants)
                {
                    var participantEntity = new RoomParticipant
                    {
                        RoomId = roomEntity.Id,
                        UserId = participant.UserId,
                        JoinedAt = participant.JoinedAt
                    };
                    await _dbContext.RoomParticipants.AddAsync(participantEntity);
                }
                await _dbContext.SaveChangesAsync();

                // Save user answers
                foreach (var answer in userAnswers.UserAnswers)
                {
                    var answerEntity = new UserAnswer
                    {
                        RoomId = roomEntity.Id,
                        UserId = answer.UserId,
                        QuestionId = answer.QuestionId,
                        Score = answer.Score,
                        TimeTakenSeconds = answer.TimeTaken,
                        IsCorrect = answer.IsCorrect,
                        SelectedOption = answer.SelectedOption
                    };
                    await _dbContext.UserAnswers.AddAsync(answerEntity);
                }
                await _dbContext.SaveChangesAsync();

                // Save leaderboard snapshot
                var leaderboardEntity = new LeaderboardSnapshot
                {
                    RoomId = roomEntity.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await _dbContext.LeaderboardSnapshots.AddAsync(leaderboardEntity);
                await _dbContext.SaveChangesAsync();

                // Save leaderboard entries
                foreach (var entry in leaderboard.Entries)
                {
                    var entryEntity = new LeaderboardSnapshot
                    {
                        UserId = entry.UserId,
                        RoomId = roomEntity.Id,
                        Score = entry.Score,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _dbContext.LeaderboardSnapshots.AddAsync(entryEntity);
                }
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Room {RoomCode} data saved to database successfully", roomCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving room {RoomCode} data to database", roomCode);
                throw;
            }
        }
    }
} 