using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SS.CMS.Services;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        private static readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public static void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public static T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public static async Task<T> GetOrCreateAsync<T>(string key, Func<ICacheEntry, Task<T>> factory)
        {
            return await _memoryCache.GetOrCreateAsync(key, factory);
        }

        public static bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}
