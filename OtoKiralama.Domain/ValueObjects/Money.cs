using System;

namespace OtoKiralama.Domain.ValueObjects
{
    public sealed record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string? currency = "TRY")
        {
            if (amount < 0m)
                throw new ArgumentOutOfRangeException(nameof(amount), "Tutar negatif olamaz.");

            Currency = string.IsNullOrWhiteSpace(currency) ? "TRY" : currency!;
            Amount = amount;
        }

        public Money Add(Money other)
        {
            if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Para birimleri eşleşmiyor.");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Multiply(int factor)
        {
            if (factor < 0)
                throw new ArgumentOutOfRangeException(nameof(factor), "Faktör negatif olamaz.");

            return new Money(Amount * factor, Currency);
        }

        public static Money Zero(string? currency = "TRY") => new(0m, currency);
        public override string ToString() => $"{Amount:0.##} {Currency}";
    }
}