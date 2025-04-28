using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QuizApp.QuizApp.Core.Interfaces;
using System.Timers;

namespace QuizApp.QuizApp.Core.Services
{
    public class QuestionTimerService : IQuestionTimerService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<QuestionTimerService> _logger;
        private const string TIMER_PREFIX = "TIMER_";
        private const string TIMER_START_PREFIX = "TIMER_START_";
        private const int QUESTION_TIMEOUT_SECONDS = 15;

        public QuestionTimerService(IMemoryCache cache, ILogger<QuestionTimerService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public void StartTimer(string roomCode, int questionIndex, Action<string, int> onTimeout)
        {
            try
            {
                _logger.LogInformation("Starting timer for room {RoomCode}, question {QuestionIndex}", roomCode, questionIndex);

                var timer = new System.Timers.Timer(QUESTION_TIMEOUT_SECONDS * 1000); // Convert to milliseconds
                timer.Elapsed += (sender, e) => OnTimerElapsed(roomCode, questionIndex, onTimeout);
                timer.AutoReset = false; // Only fire once
                timer.Start();

                // Store timer and start time in cache
                _cache.Set($"{TIMER_PREFIX}{roomCode}_{questionIndex}", timer, TimeSpan.FromSeconds(QUESTION_TIMEOUT_SECONDS + 1));
                _cache.Set($"{TIMER_START_PREFIX}{roomCode}_{questionIndex}", DateTime.UtcNow, TimeSpan.FromSeconds(QUESTION_TIMEOUT_SECONDS + 1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting timer for room {RoomCode}, question {QuestionIndex}", roomCode, questionIndex);
            }
        }

        private void OnTimerElapsed(string roomCode, int questionIndex, Action<string, int> onTimeout)
        {
            try
            {
                _logger.LogInformation("Timer elapsed for room {RoomCode}, question {QuestionIndex}", roomCode, questionIndex);
                onTimeout(roomCode, questionIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in timer elapsed for room {RoomCode}, question {QuestionIndex}", roomCode, questionIndex);
            }
        }

        public void StopTimer(string roomCode, int questionIndex)
        {
            try
            {
                _logger.LogInformation("Stopping timer for room {RoomCode}, question {QuestionIndex}", roomCode, questionIndex);

                var timer = _cache.Get<System.Timers.Timer>($"{TIMER_PREFIX}{roomCode}_{questionIndex}");
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    _cache.Remove($"{TIMER_PREFIX}{roomCode}_{questionIndex}");
                    _cache.Remove($"{TIMER_START_PREFIX}{roomCode}_{questionIndex}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping timer for room {RoomCode}, question {QuestionIndex}", roomCode, questionIndex);
            }
        }

        public void StopAllTimers(string roomCode, int currentQuestionIndex)
        {
            try
            {
                _logger.LogInformation("Stopping all timers for room {RoomCode}", roomCode);

                for (int i = 0; i <= currentQuestionIndex; i++)
                {
                    StopTimer(roomCode, i);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping all timers for room {RoomCode}", roomCode);
            }
        }

        public int GetRemainingTime(string roomCode, int questionIndex)
        {
            try
            {
                var timer = _cache.Get<System.Timers.Timer>($"{TIMER_PREFIX}{roomCode}_{questionIndex}");
                var startTime = _cache.Get<DateTime>($"{TIMER_START_PREFIX}{roomCode}_{questionIndex}");
                
                if (timer != null && timer.Enabled && startTime != default)
                {
                    var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                    return Math.Max(0, QUESTION_TIMEOUT_SECONDS - (int)elapsed);
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting remaining time for room {RoomCode}, question {QuestionIndex}", roomCode, questionIndex);
                return 0;
            }
        }
    }
} 