using System.Threading;
using System.Threading.Tasks;
using OtoKiralama.Application.Abstractions;
using OtoKiralama.Domain.ValueObjects;

namespace OtoKiralama.Infrastructure.Payments
{
    public sealed class FakePaymentService : IPaymentService
    {
        public Task<(bool Success, string? TransactionId)> ChargeAsync(Money amount, string reference, CancellationToken ct)
            => Task.FromResult((true, "TX-" + reference));
    }
}