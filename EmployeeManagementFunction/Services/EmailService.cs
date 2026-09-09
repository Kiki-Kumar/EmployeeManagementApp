using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace EmployeeManagementFunction.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(
            string recipientEmail,
            string recipientName)
        {
            var host = _configuration["Email:Host"];
            var port = int.Parse(
                _configuration["Email:Port"] ?? "2525");
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"];

            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(fromName, fromEmail));

            email.To.Add(
                new MailboxAddress(recipientName, recipientEmail));

            email.Subject = "Welcome to Employee Management";

            email.Body = new TextPart("html")
            {
                Text = $"""
                    <h2>Welcome, {recipientName}!</h2>

                    <p>Your employee profile has been created
                    successfully.</p>

                    <p>Welcome to the organization!</p>

                    <br />

                    <p>Regards,<br />
                    Employee Management Team</p>
                    """
            };

            try
            {
                using var smtpClient = new SmtpClient();

                await smtpClient.ConnectAsync(
                    host,
                    port,
                    SecureSocketOptions.None);

                await smtpClient.AuthenticateAsync(
                    username,
                    password);

                await smtpClient.SendAsync(email);

                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation(
                    "Welcome email sent successfully to {Email}.",
                    recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send welcome email to {Email}.",
                    recipientEmail);

                throw;
            }
        }
    }
}