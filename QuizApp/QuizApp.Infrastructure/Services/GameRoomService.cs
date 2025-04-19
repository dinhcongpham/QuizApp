using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Infrastructure.Data;
using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Infrastructure.Services
{
    public class GameRoomService : IGameRoomService
    {
        private readonly IMemoryCache _cache;
        private readonly IQuizService _quizService;
        private readonly IQuestionService _questionService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GameRoomService> _logger;

        public GameRoomService(
            IMemoryCache cache,
            IQuizService quizService,
            IQuestionService questionService,
            IUserRepository userRepository,
            ApplicationDbContext context,
            ILogger<GameRoomService> logger)
        {
            _cache = cache;
            _quizService = quizService;
            _questionService = questionService;
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<GameRoomDto> CreateRoomAsync(int quizId, int userId)
        {
            var quiz = await _quizService.GetByIdAsync(quizId);
            if (quiz == null) throw new Exception("Quiz not found");

            var questions = await _questionService.GetByQuizIdAsync(quizId);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var roomCode = GenerateRoomCode();
            var room = new Room
            {
                QuizId = quizId,
                HostUserId = userId,
                RoomCode = roomCode,
                Status = "Waiting",
                CreatedAt = DateTime.UtcNow
            };

            // Lưu vào cache
            var gameRoom = new GameRoomDto
            {
                RoomCode = roomCode,
                QuizId = quizId,
                HostUserId = userId,
                Questions = questions.Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    Content = q.Content,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectOption = q.CorrectOption
                }).ToList(),
                Status = "Waiting",
                CreatedAt = DateTime.UtcNow
            };

            _cache.Set($"room_{roomCode}", gameRoom, TimeSpan.FromHours(1));

            // Lưu vào database
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            return gameRoom;
        }

        public async Task<GameRoomDto> JoinRoomAsync(string roomCode, int userId)
        {
            var room = _cache.Get<GameRoomDto>($"room_{roomCode}");
            if (room == null) throw new Exception("Room not found");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            // Check if user is already in the room
            if (room.Participants.Any(p => p.UserId == userId))
                throw new Exception("User already in room");

            // Add participant to cache
            var participant = new RoomParticipantDto
            {
                UserId = userId,
                UserName = user.FullName,
                JoinedAt = DateTime.UtcNow
            };

            room.Participants.Add(participant);
            _cache.Set($"room_{roomCode}", room, TimeSpan.FromHours(1));

            // Add participant to database
            var roomEntity = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (roomEntity != null)
            {
                var roomParticipant = new RoomParticipant
                {
                    UserId = userId,
                    RoomId = roomEntity.Id,
                    JoinedAt = DateTime.UtcNow
                };

                await _context.RoomParticipants.AddAsync(roomParticipant);
                await _context.SaveChangesAsync();
            }

            return room;
        }

        public async Task<GameStateDto> StartGameAsync(string roomCode)
        {
            var room = _cache.Get<GameRoomDto>($"room_{roomCode}");
            if (room == null) throw new Exception("Room not found");

            var gameState = new GameStateDto
            {
                RoomCode = roomCode,
                CurrentQuestionIndex = 0,
                TotalQuestions = room.Questions.Count,
                StartTime = DateTime.UtcNow,
                Status = "InProgress"
            };

            // Update room status in cache
            room.Status = "InProgress";
            _cache.Set($"room_{roomCode}", room, TimeSpan.FromHours(1));

            // Update room status in database
            var roomEntity = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (roomEntity != null)
            {
                roomEntity.Status = "InProgress";
                roomEntity.StartedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            // Store game state in cache
            _cache.Set($"gamestate_{roomCode}", gameState, TimeSpan.FromHours(1));

            return gameState;
        }

        public async Task<AnswerResultDto> SubmitAnswerAsync(string roomCode, int userId, int questionId, string answer, decimal timeTaken)
        {
            var room = _cache.Get<GameRoomDto>($"room_{roomCode}");
            if (room == null) throw new Exception("Room not found");

            var question = room.Questions.FirstOrDefault(q => q.QuestionId == questionId);
            if (question == null) throw new Exception("Question not found");

            var isCorrect = answer == question.CorrectOption;
            var score = CalculateScore(isCorrect, timeTaken);

            // Store answer in cache
            var userAnswer = new UserAnswerDto
            {
                UserId = userId,
                UserName = (await _userRepository.GetByIdAsync(userId))?.FullName ?? "Unknown",
                SelectedOption = answer,
                IsCorrect = isCorrect,
                TimeTaken = timeTaken,
                Score = score
            };

            var answers = _cache.Get<List<UserAnswerDto>>($"answers_{roomCode}_{questionId}") ?? new List<UserAnswerDto>();
            answers.Add(userAnswer);
            _cache.Set($"answers_{roomCode}_{questionId}", answers, TimeSpan.FromHours(1));

            // Store answer in database
            var roomEntity = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (roomEntity != null)
            {
                var answerEntity = new UserAnswer
                {
                    UserId = userId,
                    RoomId = roomEntity.Id,
                    QuestionId = questionId,
                    SelectedOption = answer,
                    IsCorrect = isCorrect,
                    TimeTakenSeconds = timeTaken,
                    AnsweredAt = DateTime.UtcNow
                };

                await _context.UserAnswers.AddAsync(answerEntity);
                await _context.SaveChangesAsync();
            }

            return new AnswerResultDto
            {
                UserId = userId,
                QuestionId = questionId,
                IsCorrect = isCorrect,
                Score = score
            };
        }

        public async Task<LeaderboardSnapshotDto> GetLeaderboardAsync(string roomCode, int questionId)
        {
            var room = _cache.Get<GameRoomDto>($"room_{roomCode}");
            if (room == null) throw new Exception("Room not found");

            var answers = _cache.Get<List<UserAnswerDto>>($"answers_{roomCode}_{questionId}") ?? new List<UserAnswerDto>();
            var leaderboard = answers
                .GroupBy(a => a.UserId)
                .Select(g => new LeaderboardEntryDto
                {
                    UserId = g.Key,
                    UserName = g.First().UserName,
                    Score = g.Sum(a => a.Score),
                    TimeTaken = g.Average(a => a.TimeTaken)
                })
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.TimeTaken)
                .ToList();

            var snapshot = new LeaderboardSnapshotDto
            {
                RoomId = (await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode))?.Id ?? 0,
                QuestionId = questionId,
                Entries = leaderboard,
                CreatedAt = DateTime.UtcNow
            };

            // Store in cache
            _cache.Set($"leaderboard_{roomCode}_{questionId}", snapshot, TimeSpan.FromHours(1));

            // Store in database
            var roomEntity = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (roomEntity != null)
            {
                var leaderboardEntity = new LeaderboardSnapshot
                {
                    RoomId = roomEntity.Id,
                    QuestionId = questionId,
                    Score = leaderboard.FirstOrDefault()?.Score ?? 0,
                    QuestionNumber = questionId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.LeaderboardSnapshots.AddAsync(leaderboardEntity);
                await _context.SaveChangesAsync();
            }

            return snapshot;
        }

        public async Task<GameResultsDto> GetFinalResultsAsync(string roomCode)
        {
            var room = _cache.Get<GameRoomDto>($"room_{roomCode}");
            if (room == null) throw new Exception("Room not found");

            var finalResults = new GameResultsDto
            {
                RoomId = (await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode))?.Id ?? 0,
                FinalLeaderboard = new List<LeaderboardEntryDto>(),
                QuestionResults = new List<QuestionResultDto>(),
                CreatedAt = DateTime.UtcNow
            };

            // Calculate final leaderboard
            var allAnswers = new List<UserAnswerDto>();
            foreach (var question in room.Questions)
            {
                var answers = _cache.Get<List<UserAnswerDto>>($"answers_{roomCode}_{question.QuestionId}") ?? new List<UserAnswerDto>();
                allAnswers.AddRange(answers);
            }

            finalResults.FinalLeaderboard = allAnswers
                .GroupBy(a => a.UserId)
                .Select(g => new LeaderboardEntryDto
                {
                    UserId = g.Key,
                    UserName = g.First().UserName,
                    Score = g.Sum(a => a.Score),
                    TimeTaken = g.Average(a => a.TimeTaken)
                })
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.TimeTaken)
                .ToList();

            // Get detailed results for each question
            foreach (var question in room.Questions)
            {
                var answers = _cache.Get<List<UserAnswerDto>>($"answers_{roomCode}_{question.QuestionId}") ?? new List<UserAnswerDto>();
                finalResults.QuestionResults.Add(new QuestionResultDto
                {
                    QuestionId = question.QuestionId,
                    Content = question.Content,
                    CorrectOption = question.CorrectOption,
                    UserAnswers = answers
                });
            }

            // Update room status
            room.Status = "Ended";
            _cache.Set($"room_{roomCode}", room, TimeSpan.FromHours(1));

            var roomEntity = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (roomEntity != null)
            {
                roomEntity.Status = "Ended";
                roomEntity.EndedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return finalResults;
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private decimal CalculateScore(bool isCorrect, decimal timeTaken)
        {
            if (!isCorrect) return 0;
            return Math.Max(0, 1000 - (timeTaken * 10));
        }
    }
} 