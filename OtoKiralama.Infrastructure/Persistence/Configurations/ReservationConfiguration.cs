using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtoKiralama.Domain.Entities;
using OtoKiralama.Domain.ValueObjects;

namespace OtoKiralama.Infrastructure.Persistence.Configurations
{
    internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .IsRequired();

            // DateRange bir struct (değer tipi) olduğu için OwnsOne ile kullanılamaz.
            // Bunun yerine EF Core 8+ ile ComplexType kullanılabilir:

            builder.ComplexProperty(x => x.Period, period =>
            {
                period.Property(p => p.Start).HasColumnName("Start");
                period.Property(p => p.End).HasColumnName("End");
            });

            // TotalPrice (Money) owned
            var price = builder.OwnsOne(x => x.TotalPrice);
            price.Property(p => p.Amount)
                 .HasColumnName("TotalAmount")
                 .HasColumnType("decimal(18,2)");
            price.Property(p => p.Currency)
                 .HasColumnName("Currency")
                 .HasMaxLength(3); // TRY, USD, EUR
            // İstersen non-null zorunlu kıl:
            // builder.Navigation(x => x.TotalPrice).IsRequired();

            builder.Property(x => x.CarId).IsRequired();
            builder.Property(x => x.CustomerId).IsRequired();

            // Uygulanan kupon ve sigorta kodları
            builder.Property(x => x.AppliedCouponCode).HasMaxLength(32);
            builder.Property(x => x.AppliedInsuranceCode).HasMaxLength(32);

            builder.HasIndex(x => x.CarId);
            builder.HasIndex(x => x.CustomerId);
        }
    }
}
