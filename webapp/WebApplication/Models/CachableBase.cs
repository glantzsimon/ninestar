using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;

namespace K9.WebApplication.Models
{
    public abstract class CachableBase
    {
        private readonly MemoryCache _cache = MemoryCache.Default;
        // Cache results and in-flight work are shared across service instances.
        private static readonly ConcurrentDictionary<string, Lazy<object>> InFlight =
            new ConcurrentDictionary<string, Lazy<object>>(StringComparer.Ordinal);

        protected T GetOrAddToCache<T>(string cacheKey, Func<T> fetch, TimeSpan? duration = null)
        {
            var cached = _cache.Get(cacheKey);
            if (cached != null)
                return (T)cached;

            var pending = InFlight.GetOrAdd(cacheKey, key => new Lazy<object>(() =>
            {
                // A previous computation may have populated the cache after our first lookup.
                var existing = _cache.Get(key);
                if (existing != null)
                    return existing;

                var result = fetch();
                _cache.Set(key, result, DateTimeOffset.UtcNow.Add(duration ?? TimeSpan.FromMinutes(30)));
                return result;
            }, LazyThreadSafetyMode.ExecutionAndPublication));

            try
            {
                return (T)pending.Value;
            }
            finally
            {
                // Remove only this computation, never a newer one under the same key.
                // Failed computations are removed too, so a later request can retry.
                ((ICollection<KeyValuePair<string, Lazy<object>>>)InFlight)
                    .Remove(new KeyValuePair<string, Lazy<object>>(cacheKey, pending));
            }
        }

        protected void RemoveFromCache(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }
}
