using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtoKiralama.Domain.Entities;

namespace OtoKiralama.Infrastructure.Persistence.Configurations
{
    internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(64); // ← eklendi
            builder.Property(x => x.Phone).HasMaxLength(50);

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.UserId).IsUnique(); // ← eklendi
        }
    }
}