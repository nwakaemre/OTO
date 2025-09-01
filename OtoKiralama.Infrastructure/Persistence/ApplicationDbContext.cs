using Microsoft.EntityFrameworkCore;
using OtoKiralama.Domain.Entities;

namespace OtoKiralama.Infrastructure.Persistence
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<InsurancePlan> InsurancePlans => Set<InsurancePlan>();
        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public DbSet<PaymentRecord> Payments => Set<PaymentRecord>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("ApplicationUsers");
                b.HasKey(u => u.Id);
                b.Property(u => u.Email).IsRequired().HasMaxLength(256);
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
                b.Property(u => u.CreatedAt).IsRequired();
            });
        }
    }
}
