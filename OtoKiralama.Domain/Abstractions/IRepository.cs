using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OtoKiralama.Domain.Common; // AggregateRoot

namespace OtoKiralama.Domain.Abstractions
{
    public interface IRepository<T> where T : AggregateRoot
    {
        void Add(T aggregate);
        void Update(T aggregate);
        void Remove(T aggregate);

        Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<IReadOnlyList<T>> ListAsync(CancellationToken ct);

        Task<int> CountAsync(
            Func<IQueryable<T>, IQueryable<T>> queryShaper,
            CancellationToken ct);

        Task<IReadOnlyList<T>> ListAsync(
            Func<IQueryable<T>, IQueryable<T>> queryShaper,
            int skip,
            int take,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy,
            CancellationToken ct);
    }
}
