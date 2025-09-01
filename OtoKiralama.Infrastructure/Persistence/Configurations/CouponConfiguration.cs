using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtoKiralama.Domain.Entities;

namespace OtoKiralama.Infrastructure.Persistence.Configurations
{
    internal sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                   .HasMaxLength(32);

            builder.HasIndex(x => x.Code)
                   .IsUnique();

            builder.Property(x => x.Type)
                   .HasConversion<string>();

            builder.Property(x => x.Value)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Currency)
                   .HasMaxLength(10);

            builder.Property(x => x.StartsAt)
                   .HasColumnName("StartsAt");

            builder.Property(x => x.EndsAt)
                   .HasColumnName("EndsAt");

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);
        }
    }
}