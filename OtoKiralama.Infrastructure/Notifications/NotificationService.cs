using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OtoKiralama.Application.Notifications;

namespace OtoKiralama.Infrastructure.Notifications
{
    public sealed class NotificationService : INotificationService
    {
        private readonly SmtpEmailSender _emailSender;
        private readonly ConsoleSmsSender _smsSender;

        public NotificationService(ILogger<SmtpEmailSender> emailLogger, ILogger<ConsoleSmsSender> smsLogger)
        {
            _emailSender = new SmtpEmailSender(emailLogger);
            _smsSender = new ConsoleSmsSender(smsLogger);
        }

        public Task SendEmailAsync(string to, string subject, string html, CancellationToken ct)
            => _emailSender.SendAsync(to, subject, html, ct);

        public Task SendSmsAsync(string phone, string text, CancellationToken ct)
            => _smsSender.SendAsync(phone, text, ct);
    }
}
