using System;
using OtoKiralama.Domain.Common;
using OtoKiralama.Domain.Enums;

namespace OtoKiralama.Domain.Entities
{
    public class PaymentRecord : AggregateRoot
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public string IntentId { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public string Provider { get; set; } = default!;
        public PaymentStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ConfirmedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}
