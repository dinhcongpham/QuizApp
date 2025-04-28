namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IQuestionTimerService
    {
        void StartTimer(string roomCode, int questionIndex, Action<string, int> onTimeout);
        void StopTimer(string roomCode, int questionIndex);
        void StopAllTimers(string roomCode, int currentQuestionIndex);
        int GetRemainingTime(string roomCode, int questionIndex);
    }
} 