using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OtoKiralama.Infrastructure.Notifications
{
     public sealed class ConsoleSmsSender
    {
        private readonly ILogger<ConsoleSmsSender> _logger;

        public ConsoleSmsSender(ILogger<ConsoleSmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string phone, string text, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("phone zorunludur.", nameof(phone));
            text ??= string.Empty;

            _logger.LogInformation("DEV SMS -> To: {Phone}\n{Text}", phone, text);

            return Task.CompletedTask;
        }
    }
}
