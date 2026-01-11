using Microsoft.EntityFrameworkCore;

namespace AtomicHabits.Service
{
    public interface IJwtKeyProvider
    {
        Task<string> GetKeyAsync();
    }

    public class JwtKeyProvider : IJwtKeyProvider
    {
        private readonly AppDbContext _db;
        private string _cachedKey;
        private DateTime _lastFetched;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromHours(1);

        public JwtKeyProvider(AppDbContext db)
        {
            _db = db;
        }

        public async Task<string> GetKeyAsync()
        {
            if (_cachedKey != null && DateTime.UtcNow - _lastFetched < _refreshInterval)
                return _cachedKey;

            _cachedKey = await _db.JwtKeys
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Key)
                .FirstOrDefaultAsync();

            _lastFetched = DateTime.UtcNow;
            return _cachedKey!;
        }
    }

}
