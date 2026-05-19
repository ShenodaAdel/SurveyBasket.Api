using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveyBasket.Application.Services.Email;

namespace SurveyBasket.API.Health
{
    public class MailProviderHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
    {
        private readonly MailSettings _mailSettings = mailSettings.Value;
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password, cancellationToken);
                return await Task.FromResult(HealthCheckResult.Healthy("Mail provider is healthy"));
            }
            catch(Exception ex) 
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(exception:ex));
            }
        }
    }
}
