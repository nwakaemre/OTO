using System;
using OtoKiralama.Domain.ValueObjects;

namespace OtoKiralama.Domain.Events
{
    public sealed record ReservationCreatedEvent(
        Guid ReservationId,
        Guid CarId,
        Guid CustomerId,
        DateRange Period);
}