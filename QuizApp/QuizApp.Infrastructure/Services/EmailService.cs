using QuizApp.QuizApp.Core.Interfaces;
using System.Net.Mail;
using System.Net;

namespace QuizApp.QuizApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlContent)
        {
            var smtpClient = new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config["Smtp:From"]),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true,
            };
            message.To.Add(toEmail);

            await smtpClient.SendMailAsync(message);
        }
    }

}
