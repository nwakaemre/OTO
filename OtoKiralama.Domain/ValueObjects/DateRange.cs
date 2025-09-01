using System;

namespace OtoKiralama.Domain.ValueObjects
{
    public readonly record struct DateRange
    {
        public DateOnly Start { get; }
        public DateOnly End { get; }

        public DateRange(DateOnly start, DateOnly end)
        {
            if (end <= start)
                throw new ArgumentException("End, Start'tan büyük olmalıdır.", nameof(end));

            Start = start;
            End = end;
        }

        public int LengthInDays() => End.DayNumber - Start.DayNumber;

        public bool Overlaps(DateRange other)
            => !(End <= other.Start || other.End <= Start);
    }
}