using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AtomicHabits.Data
{
    public class JwtKeySeeder
    {
        private readonly AppDbContext _db;

        public JwtKeySeeder(AppDbContext db)
        {
            _db = db;
        }

        public async Task SeedInitialJwtKeyAsync()
        {
            if (await _db.JwtKeys.AnyAsync()) return;

            var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var createdAt = DateTime.UtcNow;
            var isActive = true;

            var sql = "INSERT INTO JwtKeys ([Key], CreatedAt, IsActive) VALUES (@key, @createdAt, @isActive)";
            await _db.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@key", key),
                new SqlParameter("@createdAt", createdAt),
                new SqlParameter("@isActive", isActive));
        }
    }
}
