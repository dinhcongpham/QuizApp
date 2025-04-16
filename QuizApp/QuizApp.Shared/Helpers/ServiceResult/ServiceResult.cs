namespace QuizApp.QuizApp.Shared.Helpers.ServiceResult
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static ServiceResult Fail(string message) => new() { Success = false, Message = message };
        public static ServiceResult Ok(object? data = null) => new() { Success = true, Data = data };
    }

}
