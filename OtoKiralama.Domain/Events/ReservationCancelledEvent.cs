using System;

namespace OtoKiralama.Domain.Events
{
    public sealed record ReservationCancelledEvent(
        Guid ReservationId,
        DateTime CancelledAt);
}