using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OtoKiralama.Application.Notifications;
using OtoKiralama.Domain.Entities;
using OtoKiralama.Infrastructure.Persistence;

namespace OtoKiralama.Infrastructure.BackgroundJobs
{
    // Uygulama başlarken AddHostedService<OutboxProcessor>() ile kaydedin.
    public sealed class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessor> _logger;

        // 5 sn poll, 5 * 2^Retries sn exponential backoff
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
        private const int BaseBackoffSeconds = 5;
        private const int MaxBackoffExponent = 8; // 5 * 2^8 = 1280 sn (~21 dk) üst sınır

        public OutboxProcessor(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxProcessor başlatıldı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    var now = DateTimeOffset.UtcNow;

                    // İşlenmemiş tüm mesajları getir (tracking açık kalsın).
                    var pending = await db.Set<OutboxMessage>()
                        .Where(m => m.ProcessedAt == null)
                        .OrderBy(m => m.OccurredAt)
                        .Take(100)
                        .ToListAsync(stoppingToken);

                    if (pending.Count == 0)
                    {
                        await Task.Delay(PollInterval, stoppingToken);
                        continue;
                    }

                    foreach (var msg in pending)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        // Exponential backoff: OccurredAt + 5 * 2^Retries sn’ye kadar bekle
                        var exponent = Math.Min(MaxBackoffExponent, Math.Max(0, msg.Retries));
                        var delaySeconds = BaseBackoffSeconds * (int)Math.Pow(2, exponent);
                        var dueAt = msg.OccurredAt.AddSeconds(delaySeconds);

                        if (dueAt > now)
                        {
                            // Daha zamanı gelmemiş; sonraki döngüde bakacağız.
                            continue;
                        }

                        try
                        {
                            await ProcessMessageAsync(msg, notificationService, stoppingToken);

                            msg.ProcessedAt = DateTimeOffset.UtcNow;
                            msg.Error = null;

                            await db.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            msg.Retries += 1;
                            msg.Error = Truncate(ex.ToString(), 2048);

                            _logger.LogWarning(ex,
                                "OutboxMessage işlenemedi. Id={Id}, Type={Type}, Retries={Retries}",
                                msg.Id, msg.Type, msg.Retries);

                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // normal kapanış
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OutboxProcessor döngüsünde beklenmeyen hata.");
                    // Gürültüyü azaltmak için küçük bir gecikme ekleyelim.
                }

                try
                {
                    await Task.Delay(PollInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // normal kapanış
                }
            }

            _logger.LogInformation("OutboxProcessor durduruldu.");
        }

        private async Task ProcessMessageAsync(OutboxMessage msg, INotificationService notificationService, CancellationToken ct)
        {
            // Type alanına göre yönlendir. Beklenen tipler: "email" | "sms"
            var type = (msg.Type ?? string.Empty).Trim();

            if (type.Equals("email", StringComparison.OrdinalIgnoreCase))
            {
                var payload = Deserialize<EmailPayload>(msg.Payload);
                if (payload is null)
                    throw new InvalidOperationException("Email payload çözümlenemedi.");

                await notificationService.SendEmailAsync(payload.To, payload.Subject ?? string.Empty, payload.Html ?? string.Empty, ct);
                return;
            }

            if (type.Equals("sms", StringComparison.OrdinalIgnoreCase))
            {
                var payload = Deserialize<SmsPayload>(msg.Payload);
                if (payload is null)
                    throw new InvalidOperationException("Sms payload çözümlenemedi.");

                await notificationService.SendSmsAsync(payload.Phone, payload.Text ?? string.Empty, ct);
                return;
            }

            throw new NotSupportedException($"Desteklenmeyen OutboxMessage Type: '{msg.Type}'.");
        }

        // Truncate yardımcı metodunu ekleyin
        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        // OutboxProcessor.cs dosyasına ekleyin (örn. sınıfın en altına veya uygun bir yere)
        private static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }

    // Dosyanın başına veya uygun bir namespace'e ekleyin
    public class SmsPayload
    {
        public string Phone { get; set; }
        public string Text { get; set; }
    }

    // OutboxProcessor.cs dosyasına ekleyin (SmsPayload ile aynı yere)
    public class EmailPayload
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
    }
}
