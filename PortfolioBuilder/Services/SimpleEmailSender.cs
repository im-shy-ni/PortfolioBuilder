
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using System.Threading.Tasks;
using System;

namespace PortfolioBuilder.Services
{
    public class SimpleEmailSender : IEmailSender
    {
        private readonly string _folder;
        public SimpleEmailSender()
        {
            _folder = Path.Combine(Directory.GetCurrentDirectory(), "Emails");
            if (!Directory.Exists(_folder)) Directory.CreateDirectory(_folder);
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var file = Path.Combine(_folder, $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}.html");
            File.WriteAllText(file, $"To: {email}\nSubject: {subject}\n\n{htmlMessage}");
            Console.WriteLine($"Email written to: {file}");
            return Task.CompletedTask;
        }
    }
}
