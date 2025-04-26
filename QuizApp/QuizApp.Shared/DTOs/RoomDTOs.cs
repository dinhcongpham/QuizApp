using System.ComponentModel.DataAnnotations;

namespace QuizApp.QuizApp.Shared.DTOs
{
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
