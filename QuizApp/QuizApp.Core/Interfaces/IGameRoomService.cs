using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IGameRoomService
    {
        Task<GameRoomDto> CreateRoomAsync(int quizId, int userId);
        Task<GameRoomDto> JoinRoomAsync(string roomCode, int userId);
        Task<GameStateDto> StartGameAsync(string roomCode);
        Task<AnswerResultDto> SubmitAnswerAsync(string roomCode, int userId, int questionId, string answer, decimal timeTaken);
        Task<LeaderboardSnapshotDto> GetLeaderboardAsync(string roomCode, int questionId);
        Task<GameResultsDto> GetFinalResultsAsync(string roomCode);
    }
} 