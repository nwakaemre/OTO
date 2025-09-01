using System;
using OtoKiralama.Domain.Common;

namespace OtoKiralama.Domain.Entities
{
    public class Branch : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        protected Branch() { }

        private Branch(Guid id, string name, string city, string address, bool isActive)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id boş olamaz.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name boş olamaz.", nameof(name));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City boş olamaz.", nameof(city));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address boş olamaz.", nameof(address));

            Id = id;
            Name = name.Trim();
            City = city.Trim();
            Address = address.Trim();
            IsActive = isActive;
        }

        public static Branch Create(Guid id, string name, string city, string address, bool isActive = true)
            => new(id, name, city, address, isActive);

        public void UpdateDetails(string name, string city, string address)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name boş olamaz.", nameof(name));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City boş olamaz.", nameof(city));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address boş olamaz.", nameof(address));

            Name = name.Trim();
            City = city.Trim();
            Address = address.Trim();
            Touch();
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            Touch();
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            Touch();
        }
    }
}