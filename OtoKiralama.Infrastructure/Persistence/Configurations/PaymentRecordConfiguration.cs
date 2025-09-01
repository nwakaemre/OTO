// OtoKiralama.Infrastructure/Persistence/Configurations/PaymentRecordConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtoKiralama.Domain.Entities;

namespace OtoKiralama.Infrastructure.Persistence.Configurations
{
    internal sealed class PaymentRecordConfiguration : IEntityTypeConfiguration<PaymentRecord>
    {
        public void Configure(EntityTypeBuilder<PaymentRecord> builder)
        {
            builder.ToTable("PaymentRecords");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IntentId)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(x => x.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Currency)
                   .IsRequired()
                   .HasMaxLength(3);

            builder.Property(x => x.Provider)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.ConfirmedAt);

            builder.Property(x => x.FailureReason)
                   .HasMaxLength(512);

            builder.HasIndex(x => x.IntentId).IsUnique();
            builder.HasIndex(x => x.ReservationId);

            builder.HasOne<Reservation>()
                   .WithMany()
                   .HasForeignKey(x => x.ReservationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
