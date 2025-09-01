using System;
using System.Collections.Generic;

namespace OtoKiralama.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } // protected yerine public
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        private readonly List<object> _domainEvents = new();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity()
        {
            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        protected void Touch() => UpdatedAt = DateTime.UtcNow;

        public void AddDomainEvent(object ev)
        {
            if (ev is null) throw new ArgumentNullException(nameof(ev));
            _domainEvents.Add(ev);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}