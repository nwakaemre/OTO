using System;
using System.Threading;
using System.Threading.Tasks;
using OtoKiralama.Application.Payments;

namespace OtoKiralama.Infrastructure.Payments
{
    public sealed class FakePaymentGateway : IPaymentGateway
    {
        public Task<(string intentId, string? clientSecret)> CreateIntentAsync(Guid reservationId, decimal amount, string currency, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Task.FromCanceled<(string intentId, string? clientSecret)>(ct);

            var intentId = $"pay_{Guid.NewGuid():N}";
            var clientSecret = $"cs_test_{Guid.NewGuid():N}";
            return Task.FromResult((intentId, clientSecret));
        }

        public Task<bool> ConfirmAsync(string intentId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return Task.FromCanceled<bool>(ct);

            // Demo için her zaman başarılı
            return Task.FromResult(true);
        }
    }
}
