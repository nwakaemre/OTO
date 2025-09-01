using System;
using OtoKiralama.Domain.Common;
using OtoKiralama.Domain.ValueObjects;

namespace OtoKiralama.Domain.Entities
{
    public sealed class InsurancePlan : AggregateRoot
    {
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public Money DailyFee { get; private set; } = default!;
        public string? CoverageNote { get; private set; }
        public bool IsActive { get; private set; }

        private InsurancePlan() { } // EF Core için

        private InsurancePlan(Guid id, string code, string name, Money dailyFee, string? coverageNote, bool isActive)
        {
            Id = id;
            Code = NormalizeCode(code);
            Name = NormalizeName(name);
            DailyFee = ValidateDailyFee(dailyFee);
            CoverageNote = NormalizeCoverageNote(coverageNote);
            IsActive = isActive;
        }

        public static InsurancePlan Create(Guid id, string code, string name, Money dailyFee, string? coverageNote = null, bool isActive = true)
            => new(id, code, name, dailyFee, coverageNote, isActive);

        public void Update(string name, Money dailyFee, string? coverageNote, bool isActive)
        {
            Name = NormalizeName(name);
            DailyFee = ValidateDailyFee(dailyFee);
            CoverageNote = NormalizeCoverageNote(coverageNote);
            IsActive = isActive;
            Touch();
        }

        public override string ToString()
            => $"{Code} - {Name} | Günlük: {DailyFee} | {(IsActive ? "Aktif" : "Pasif")}";

        private static string NormalizeCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code boş olamaz.", nameof(code));

            var norm = code.Trim().ToUpperInvariant();
            if (norm.Length > 32)
                throw new ArgumentOutOfRangeException(nameof(code), "Code en fazla 32 karakter olmalıdır.");

            return norm;
        }

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name boş olamaz.", nameof(name));

            var norm = name.Trim();
            if (norm.Length > 64)
                throw new ArgumentOutOfRangeException(nameof(name), "Name en fazla 64 karakter olmalıdır.");

            return norm;
        }

        private static string? NormalizeCoverageNote(string? note)
        {
            if (string.IsNullOrWhiteSpace(note))
                return null;

            var norm = note.Trim();
            if (norm.Length > 256)
                throw new ArgumentOutOfRangeException(nameof(note), "CoverageNote en fazla 256 karakter olmalıdır.");

            return norm;
        }

        private static Money ValidateDailyFee(Money dailyFee)
        {
            if (dailyFee is null)
                throw new ArgumentNullException(nameof(dailyFee));

            if (dailyFee.Amount < 0m)
                throw new ArgumentOutOfRangeException(nameof(dailyFee), "DailyFee.Amount negatif olamaz.");

            return dailyFee;
        }
    }
}
