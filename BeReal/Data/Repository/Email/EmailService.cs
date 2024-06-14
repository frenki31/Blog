using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BeReal.Data.Repository.Email
{
    public class EmailService : IEmailService
    {
        private readonly ISendGridClient _client;
        private readonly SendGridSettings _settings;

        public EmailService(ISendGridClient client, IOptions<SendGridSettings> settings)
        {
            _settings = settings.Value;
            _client = client;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new SendGridMessage()
            {
                From = new EmailAddress(_settings.FromEmail, _settings.EmailName),
                Subject = subject,
                HtmlContent = message
            };
            mailMessage.AddTo(email);
            await _client.SendEmailAsync(mailMessage);
        }
    }
}
