using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OtoKiralama.Infrastructure.Persistence
{
    public static class DevDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Sadece migration uygula
            await db.Database.MigrateAsync();

            // ❌ Hiçbir veri eklenmeyecek
        }
    }
}
