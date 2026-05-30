using Microsoft.Extensions.Caching.Memory;

namespace ClickExpress.BusinessLogic.Helpers
{
    public interface ICacheService
    {
        T? Get<T>(string key) where T : class;
        void Set<T>(string key, T value, TimeSpan ttl) where T : class;
        void Remove(string key);
        void RemoveByPrefix(string prefix);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _keys = [];
        private readonly Lock _lock = new();

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T? value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan ttl) where T : class
        {
            var opts = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl,
                SlidingExpiration = null,
            };

            opts.RegisterPostEvictionCallback((k, _, _, _) =>
            {
                lock (_lock) _keys.Remove(k.ToString()!);
            });

            _cache.Set(key, value, opts);

            lock (_lock)
            {
                if (!_keys.Contains(key)) _keys.Add(key);
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            lock (_lock) _keys.Remove(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            List<string> toRemove;
            lock (_lock) toRemove = _keys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in toRemove) _cache.Remove(key);
            lock (_lock) _keys.RemoveAll(k => k.StartsWith(prefix));
        }
    }
}
