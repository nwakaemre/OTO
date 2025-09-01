using System;
using OtoKiralama.Domain.Common;
using OtoKiralama.Domain.Enums;

namespace OtoKiralama.Domain.Entities
{
    public class Coupon : AggregateRoot
    {
        public string Code { get; private set; } = string.Empty;
        public CouponType Type { get; private set; }
        public decimal Value { get; private set; }
        public string? Currency { get; private set; }
        public int? MinDays { get; private set; }
        public DateOnly? StartsAt { get; private set; }
        public DateOnly? EndsAt { get; private set; }
        public bool IsActive { get; private set; }
        public int? MaxUses { get; private set; }
        public int UsedCount { get; private set; }

        protected Coupon() { }

        private Coupon(
            Guid id,
            string code,
            CouponType type,
            decimal value,
            string? currency,
            int? minDays,
            DateOnly? startsAt,
            DateOnly? endsAt,
            bool isActive,
            int? maxUses,
            int usedCount)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id boş olamaz.", nameof(id));
            Id = id;

            Code = NormalizeCode(code);
            Type = type;

            // Tip ve değer doğrulama
            switch (type)
            {
                case CouponType.Percent:
                    if (value < 0m || value > 100m)
                        throw new ArgumentOutOfRangeException(nameof(value), "Yüzde 0-100 aralığında olmalıdır.");
                    Value = value;
                    Currency = null; // yüzde indirimi para birimi gerektirmez
                    break;

                case CouponType.Amount:
                    if (value <= 0m)
                        throw new ArgumentOutOfRangeException(nameof(value), "Tutar pozitif olmalıdır.");
                    Value = value;
                    if (string.IsNullOrWhiteSpace(currency))
                        throw new ArgumentException("Para birimi zorunludur.", nameof(currency));
                    Currency = currency.Trim().ToUpperInvariant();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Geçersiz kupon tipi.");
            }

            if (minDays.HasValue && minDays.Value < 1)
                throw new ArgumentOutOfRangeException(nameof(minDays), "MinDays en az 1 olmalıdır.");
            MinDays = minDays;

            if (startsAt.HasValue && endsAt.HasValue && endsAt.Value <= startsAt.Value)
                throw new ArgumentException("EndsAt, StartsAt'tan büyük olmalıdır.", nameof(endsAt));
            StartsAt = startsAt;
            EndsAt = endsAt;

            if (maxUses.HasValue && maxUses.Value < 1)
                throw new ArgumentOutOfRangeException(nameof(maxUses), "MaxUses en az 1 olmalıdır.");
            if (usedCount < 0)
                throw new ArgumentOutOfRangeException(nameof(usedCount), "UsedCount negatif olamaz.");
            if (maxUses.HasValue && usedCount > maxUses.Value)
                throw new ArgumentException("UsedCount, MaxUses değerini aşamaz.", nameof(usedCount));
            MaxUses = maxUses;
            UsedCount = usedCount;

            IsActive = isActive;
        }

        public static Coupon Create(
            Guid id,
            string code,
            CouponType type,
            decimal value,
            string? currency = null,
            int? minDays = null,
            DateOnly? startsAt = null,
            DateOnly? endsAt = null,
            bool isActive = true,
            int? maxUses = null)
            => new(
                id,
                code,
                type,
                value,
                currency,
                minDays,
                startsAt,
                endsAt,
                isActive,
                maxUses,
                usedCount: 0);

        private static string NormalizeCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code zorunludur.", nameof(code));
            return code.Trim().ToUpperInvariant();
        }

        public void UpdateDetails(
            decimal value,
            string? currency,
            int? minDays,
            DateOnly? startsAt,
            DateOnly? endsAt,
            bool isActive,
            int? maxUses)
        {
            // Gerekli validasyonları burada yapabilirsiniz.
            Value = value;
            Currency = currency;
            MinDays = minDays;
            StartsAt = startsAt;
            EndsAt = endsAt;
            IsActive = isActive;
            MaxUses = maxUses;
            Touch(); // UpdatedAt güncellemesi için
        }
    }
}
