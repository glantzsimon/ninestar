using System;
using System.Runtime.Caching;

namespace K9.WebApplication.Models
{
    public abstract class CachableBase
    {
        private readonly MemoryCache _cache = MemoryCache.Default;

        protected T GetOrAddToCache<T>(string cacheKey, Func<T> fetch, TimeSpan? duration = null)
        {
            if (_cache.Contains(cacheKey))
            {
                return (T)_cache.Get(cacheKey);
            }

            var result = fetch();
            _cache.Set(cacheKey, result, DateTimeOffset.UtcNow.Add(duration ?? TimeSpan.FromMinutes(30)));
            return result;
        }

        protected void RemoveFromCache(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }
}