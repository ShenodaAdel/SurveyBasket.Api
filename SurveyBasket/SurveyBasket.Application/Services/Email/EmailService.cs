using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SurveyBasket.Application.Services.Email
{
    public class EmailService(IOptions<MailSettings> mailSettings , ILogger<EmailService> logger) : IEmailService
    {
        private readonly MailSettings _mailSettings = mailSettings.Value;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = subject,

            };
            message.To.Add(MailboxAddress.Parse(email));
            
            var builder = new BodyBuilder { HtmlBody = htmlMessage };

            message.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
