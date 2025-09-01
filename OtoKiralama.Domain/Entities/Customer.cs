using System;
using OtoKiralama.Domain.Common;

namespace OtoKiralama.Domain.Entities
{
    public class Customer : AggregateRoot
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string UserId { get; private set; } = string.Empty; // ← eklendi
        public string? Phone { get; private set; }
        public DateOnly DateOfBirth { get; private set; }

        public string FullName => string.Concat(FirstName, " ", LastName).Trim();

        protected Customer() { }

        private Customer(Guid id, string firstName, string lastName, string email, string userId, string? phone, DateOnly dateOfBirth)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id boş olamaz.", nameof(id));
            SetName(firstName, lastName);
            SetUserId(userId);          // ← önce UserId
            SetEmail(email);            // ← sonra Email

            Id = id;
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            DateOfBirth = dateOfBirth;
        }

        [Obsolete("UserId gerekli. Lütfen userId parametreli aşırı yüklemeyi kullanın.")]
        public static Customer Create(Guid id, string firstName, string lastName, string email, string? phone, DateOnly dateOfBirth)
            => new(id, firstName, lastName, email, email, phone, dateOfBirth); // varsayılan: UserId = Email

        public static Customer Create(Guid id, string firstName, string lastName, string email, string userId, string? phone, DateOnly dateOfBirth)
            => new(id, firstName, lastName, email, userId, phone, dateOfBirth);

        public void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Trim().Length < 2)
                throw new ArgumentException("FirstName en az 2 karakter olmalıdır.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Trim().Length < 2)
                throw new ArgumentException("LastName en az 2 karakter olmalıdır.", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Touch();
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email boş olamaz.", nameof(email));
            var e = email.Trim();
            if (!e.Contains('@') || !e.Contains('.'))
                throw new ArgumentException("Email formatı geçersiz.", nameof(email));

            if (string.IsNullOrWhiteSpace(UserId))
                throw new InvalidOperationException("UserId atanmadıkça Email ayarlanamaz."); // tutarlılık

            Email = e;
            Touch();
        }

        public void SetUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId boş olamaz.", nameof(userId));

            UserId = userId.Trim();
            Touch();
        }

        public void SetPhone(string? phone)
        {
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            Touch();
        }
    }
}