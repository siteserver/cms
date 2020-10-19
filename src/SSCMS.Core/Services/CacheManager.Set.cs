using System;
using CacheManager.Core;

namespace SSCMS.Core.Services
{
    public partial class CacheManager<TCacheValue>
    {
        public void AddOrUpdateSliding(string key, TCacheValue value, int minutes)
        {
            var cacheItem = new CacheItem<TCacheValue>(key, value, ExpirationMode.Sliding, TimeSpan.FromMinutes(minutes));
            _cacheManager.AddOrUpdate(cacheItem, _ => value);
        }

        public void AddOrUpdateAbsolute(string key, TCacheValue value, int minutes)
        {
            var cacheItem = new CacheItem<TCacheValue>(key, value, ExpirationMode.Absolute, TimeSpan.FromMinutes(minutes));
            _cacheManager.AddOrUpdate(cacheItem, _ => value);
        }

        public void AddOrUpdate(string key, TCacheValue value)
        {
            _cacheManager.AddOrUpdate(key, value, _ => value);
        }
    }
}
