using QuizApp.QuizApp.Shared.DTOs;

namespace QuizApp.QuizApp.Core.Interfaces
{
    public interface IGameRoomService
    {
        Task<GameRoomDto> CreateRoomAsync(int quizId, int userId);
        Task<GameRoomDto> JoinRoomAsync(string roomCode, int userId);
        Task<(GameRoomDto, GameStateDto, LeaderboardSnapshotDto)> JoinRoomWhenProgressAsync(string roomCode, int userId);
        Task<GameStateDto> StartGameAsync(string roomCode);
        Task SubmitAnswerAsync(string roomCode, int userId, int questionId, string answer, decimal timeTaken);
        Task<LeaderboardSnapshotDto> GetLeaderboardAsync(string roomCode, int questionId);
        Task<IEnumerable<string>> GetUserRoomsAsync(string connectionId);
        Task<GameStateDto?> GetGameStateAsync(string roomCode);
        Task UpdateGameStateAsync(string roomCode, GameStateDto gameState);
        Task CleanupRoomAsync(string roomCode);
        Task<GameRoomDto?> GetRoomAsync(string roomCode);
        Task SaveRoomDataToDatabaseAsync(string roomCode);
    }
} 