using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OtoKiralama.Domain.Common;

namespace OtoKiralama.Infrastructure.Persistence.Interceptors
{
    internal sealed class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetAudit(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            SetAudit(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void SetAudit(DbContext? context)
        {
            if (context is null) return;

            var nowDt = DateTime.UtcNow;
            var nowDto = DateTimeOffset.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                var createdProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == nameof(BaseEntity.CreatedAt));
                var updatedProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == nameof(BaseEntity.UpdatedAt));

                if (entry.State == EntityState.Added)
                {
                    if (createdProp is not null)
                        createdProp.CurrentValue = createdProp.Metadata.ClrType == typeof(DateTimeOffset) || createdProp.Metadata.ClrType == typeof(DateTimeOffset?)
                            ? (object)nowDto
                            : nowDt;

                    if (updatedProp is not null)
                        updatedProp.CurrentValue = updatedProp.Metadata.ClrType == typeof(DateTimeOffset) || updatedProp.Metadata.ClrType == typeof(DateTimeOffset?)
                            ? (object)nowDto
                            : nowDt;
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (updatedProp is not null)
                        updatedProp.CurrentValue = updatedProp.Metadata.ClrType == typeof(DateTimeOffset) || updatedProp.Metadata.ClrType == typeof(DateTimeOffset?)
                            ? (object)nowDto
                            : nowDt;
                }
            }
        }
    }
}