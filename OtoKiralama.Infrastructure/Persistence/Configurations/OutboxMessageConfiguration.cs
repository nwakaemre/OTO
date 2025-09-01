// OtoKiralama.Infrastructure/Persistence/Configurations/OutboxMessageConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtoKiralama.Domain.Entities;

namespace OtoKiralama.Infrastructure.Persistence.Configurations
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> b)
        {
            b.ToTable("OutboxMessages");
            b.HasKey(x => x.Id);
            b.Property(x => x.Type).IsRequired().HasMaxLength(128);
            b.Property(x => x.Payload).IsRequired();
            b.Property(x => x.OccurredAt).IsRequired();
            b.Property(x => x.ProcessedAt);
            b.Property(x => x.Retries).IsRequired().HasDefaultValue(0);
            b.Property(x => x.Error).HasMaxLength(2048);
        }
    }
}
