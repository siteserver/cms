using System;
using CacheManager.Core;

namespace SS.CMS.Core
{
    public static class CacheUtils
    {
        private const string CachePrefix = "CacheUtils";
        private static readonly ICacheManager<object> Cache;

        static CacheUtils()
        {
            Cache = CacheFactory.Build(settings => settings
                .WithMicrosoftMemoryCacheHandle()
                .WithExpiration(ExpirationMode.None, TimeSpan.Zero));
        }

        public static void ClearAll()
        {
            Cache.Clear();
        }

        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        public static void Insert(string key, object obj)
        {
            InnerInsert(key, obj, null, TimeSpan.Zero);
        }

        public static void Insert(string key, object obj, string filePath)
        {
            InnerInsert(key, obj, filePath, TimeSpan.Zero);
        }

        public static void Insert(string key, object obj, TimeSpan timeSpan, string filePath)
        {
            InnerInsert(key, obj, filePath, timeSpan);
        }

        public static void InsertHours(string key, object obj, int hours)
        {
            InnerInsert(key, obj, null, TimeSpan.FromHours(hours));
        }

        public static void InsertMinutes(string key, object obj, int minutes)
        {
            InnerInsert(key, obj, null, TimeSpan.FromMinutes(minutes));
        }

        private static void InnerInsert(string key, object obj, string filePath, TimeSpan timeSpan)
        {
            if (Cache == null) return;

            //var cacheManager = await Caching.GetCacheManagerAsync();
            //cacheManager.Add(new CacheItem<object>())

            if (string.IsNullOrEmpty(key)) return;

            Cache.Remove(key);
            if (obj != null)
            {
                var item = timeSpan != TimeSpan.Zero
                    ? new CacheItem<object>(key, obj, ExpirationMode.Sliding, timeSpan)
                    : new CacheItem<object>(key, obj);

                Cache.Add(item);
                //Cache.Insert(key, obj, string.IsNullOrEmpty(filePath) ? null : new CacheDependency(filePath), Cache.NoAbsoluteExpiration, timeSpan, CacheItemPriority.Normal, null);
            }
        }

        public static object Get(string key)
        {
            var item = Cache.GetCacheItem(key);
            return item?.Value;
        }

        public static T Get<T>(string key) where T : class
        {
            return Cache.Get(key) as T;
        }

        public static bool Exists(string key)
        {
            return Cache.Exists(key);
        }

        public static string GetCacheKey(string nameofClass, params string[] values)
        {
            var key = $"{CachePrefix}.{nameofClass}";
            if (values == null || values.Length <= 0) return key;
            foreach (var t in values)
            {
                key += "." + t;
            }
            return key;
        }
    }
}