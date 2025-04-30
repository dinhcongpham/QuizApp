namespace QuizApp.QuizApp.Shared.Helpers.PasswordHash
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyHashedPassword(string password, string hashedPassword);
        string GenerateRandomPassword(int length);
    }
}
