using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtoKiralama.Domain.Entities;

namespace OtoKiralama.Infrastructure.Persistence.Configurations
{
    internal sealed class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Brand).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Model).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Year).IsRequired(false);

            builder.Property(x => x.FuelType).HasConversion<string>().IsRequired();
            builder.Property(x => x.TransmissionType).HasConversion<string>().IsRequired();

            // DailyPrice (Money) owned
            builder.OwnsOne(x => x.DailyPrice, o =>
            {
                o.Property(p => p.Amount)
                 .HasColumnName("DailyPriceAmount")
                 .HasColumnType("decimal(18,2)"); // ← Uyarıyı giderir

                o.Property(p => p.Currency)
                 .HasColumnName("DailyPriceCurrency")
                 .HasMaxLength(3); // TRY, USD, EUR
            });
            // İstersen zorunlu yap:
            // builder.Navigation(x => x.DailyPrice).IsRequired();

            builder.Property(x => x.BranchId).IsRequired();

            builder.HasIndex(x => new { x.Brand, x.Model, x.Year });
        }
    }
}
