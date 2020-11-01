using System;
using CacheManager.Core;

namespace SSCMS.Core.Services
{
    public partial class CacheManager
    {
        public void AddOrUpdateSliding<T>(string key, T value, int minutes)
        {
            var cacheItem = new CacheItem<object>(key, value, ExpirationMode.Sliding, TimeSpan.FromMinutes(minutes));
            _cacheManager.AddOrUpdate(cacheItem, _ => value);
        }

        public void AddOrUpdateAbsolute<T>(string key, T value, int minutes)
        {
            var cacheItem = new CacheItem<object>(key, value, ExpirationMode.Absolute, TimeSpan.FromMinutes(minutes));
            _cacheManager.AddOrUpdate(cacheItem, _ => value);
        }

        public void AddOrUpdate<T>(string key, T value)
        {
            _cacheManager.AddOrUpdate(key, value, _ => value);
        }
    }
}
