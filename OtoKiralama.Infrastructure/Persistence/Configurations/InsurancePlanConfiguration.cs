using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtoKiralama.Domain.Entities;

namespace OtoKiralama.Infrastructure.Persistence.Configurations
{
    internal sealed class InsurancePlanConfiguration : IEntityTypeConfiguration<InsurancePlan>
    {
        public void Configure(EntityTypeBuilder<InsurancePlan> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.HasIndex(x => x.Code)
                   .IsUnique();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(x => x.CoverageNote)
                   .HasMaxLength(256);

            builder.OwnsOne(x => x.DailyFee, o =>
            {
                o.Property(p => p.Amount)
                 .HasColumnName("DailyFeeAmount")
                 .HasColumnType("decimal(18,2)");

                o.Property(p => p.Currency)
                 .HasColumnName("DailyFeeCurrency")
                 .HasMaxLength(10);
            });

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);
        }
    }
}
