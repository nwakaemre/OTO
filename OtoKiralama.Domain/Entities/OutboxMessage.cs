using System;
using OtoKiralama.Domain.Common;

namespace OtoKiralama.Domain.Entities
{
    public class OutboxMessage : AggregateRoot
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTimeOffset OccurredAt { get; set; }
        public DateTimeOffset? ProcessedAt { get; set; }
        public int Retries { get; set; }
        public string? Error { get; set; }
    }
}
