using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OtoKiralama.Application.Abstractions.Identity;
using OtoKiralama.Domain.Entities;
using OtoKiralama.Infrastructure.Persistence;

namespace OtoKiralama.Infrastructure.Identity
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _db;
        private readonly JwtTokenGenerator _jwt;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _signingKey;

        public IdentityService(ApplicationDbContext db, JwtTokenGenerator jwt, IConfiguration config)
        {
            _db = db;
            _jwt = jwt;
            _issuer = config["Jwt:Issuer"]!;
            _audience = config["Jwt:Audience"]!;
            _signingKey = config["Jwt:Key"]!;
        }

        public async Task<(string Token, Guid CustomerId)> RegisterAsync(string email, string password, CancellationToken ct)
        {
            if (await _db.Set<ApplicationUser>().AnyAsync(x => x.Email == email, ct))
                throw new InvalidOperationException("Email already exists");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = Hash(password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Add(user);

            // 🔑 ApplicationUser ile aynı Id’ye sahip bir Customer kaydı oluştur
            if (!await _db.Customers.AnyAsync(c => c.Id == user.Id, ct))
            {
                var first = "App";
                var last  = "User";
                var dob   = new DateOnly(1990, 1, 1);
                var customer = Customer.Create(user.Id, first, last, email, user.Id.ToString(), null, dob);
                _db.Customers.Add(customer);
            }

            await _db.SaveChangesAsync(ct);

            var token = _jwt.GenerateToken(user.Id, email, DateTime.UtcNow.AddHours(1), _issuer, _audience, _signingKey);
            return (token, user.Id);
        }

        public async Task<(string Token, Guid CustomerId)> LoginAsync(string email, string password, CancellationToken ct)
        {
            var user = await _db.Set<ApplicationUser>().FirstOrDefaultAsync(x => x.Email == email, ct)
                ?? throw new InvalidOperationException("Invalid email or password");

            if (user.PasswordHash != Hash(password))
                throw new InvalidOperationException("Invalid email or password");

            // 🔑 Customer kaydı yoksa oluştur (id eşit olmalı)
            if (!await _db.Customers.AnyAsync(c => c.Id == user.Id, ct))
            {
                var first = "App";
                var last  = "User";
                var dob   = new DateOnly(1990, 1, 1);
                var customer = Customer.Create(user.Id, first, last, email, user.Id.ToString(), null, dob);
                _db.Customers.Add(customer);
                await _db.SaveChangesAsync(ct);
            }

            var token = _jwt.GenerateToken(user.Id, email, DateTime.UtcNow.AddHours(1), _issuer, _audience, _signingKey);
            return (token, user.Id);
        }

        private static string Hash(string input)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
