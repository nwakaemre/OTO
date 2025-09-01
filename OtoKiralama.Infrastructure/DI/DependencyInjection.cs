using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OtoKiralama.Domain.Abstractions;
using OtoKiralama.Infrastructure.Persistence;
using OtoKiralama.Infrastructure.Persistence.Interceptors;
using OtoKiralama.Infrastructure.Persistence.Repositories;
using OtoKiralama.Application.Abstractions;
using OtoKiralama.Application.Abstractions.Messaging;
using OtoKiralama.Application.Common.Pricing; // ← eklendi

namespace OtoKiralama.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> dbOptions)
        {
            services.AddSingleton<AuditableEntityInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                dbOptions(options);
                options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
            });

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPricingService, PricingService>(); // ← eklendi
            services.AddScoped<IPaymentService, OtoKiralama.Infrastructure.Payments.FakePaymentService>();

            return services;
        }
    }
}