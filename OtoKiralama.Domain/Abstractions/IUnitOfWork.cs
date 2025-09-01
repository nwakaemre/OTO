using System.Threading;
using System.Threading.Tasks;

namespace OtoKiralama.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}