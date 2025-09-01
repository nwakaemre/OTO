using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OtoKiralama.Infrastructure.Notifications
{
    public sealed class SmtpEmailSender
    {
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(ILogger<SmtpEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string html, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentException("to zorunludur.", nameof(to));
            if (string.IsNullOrWhiteSpace(subject)) subject = "(no-subject)";
            html ??= string.Empty;

            // DEV: Konsola/Log'a yaz. Gerekirse SmtpClient ile gönderim eklenebilir.
            _logger.LogInformation("DEV Email -> To: {To}, Subject: {Subject}\n{Body}", to, subject, html);

            return Task.CompletedTask;
        }
    }
}
