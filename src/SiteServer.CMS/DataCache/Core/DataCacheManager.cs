using System;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache.Core
{
    public static class DataCacheManager
    {
        private const string CachePrefix = "DataCacheManager";

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

        public static T Get<T>(string cacheKey) where T : class
        {
            return CacheUtils.Get<T>(cacheKey);
        }

        public static void Insert(string cacheKey, object value)
        {
            CacheUtils.Insert(cacheKey, value);
        }

        public static void Insert(string cacheKey, object value, string filePath)
        {
            CacheUtils.Insert(cacheKey, value, filePath);
        }

        public static void Insert(string key, object obj, TimeSpan timeSpan, string filePath)
        {
            CacheUtils.Insert(key, obj, timeSpan, filePath);
        }

        public static void InsertHours(string cacheKey, object value, int hours)
        {
            CacheUtils.InsertHours(cacheKey, value, hours);
        }

        public static void InsertMinutes(string cacheKey, object value, int minutes)
        {
            CacheUtils.InsertMinutes(cacheKey, value, minutes);
        }

        public static void Remove(string cacheKey)
        {
            CacheUtils.Remove(cacheKey);
        }

        public static void RemoveByClassName(string className)
        {
            CacheUtils.RemoveByStartString(GetCacheKey(className));
        }

        public static void RemoveByPrefix(string prefix)
        {
            CacheUtils.RemoveByStartString(prefix);
        }
    }
}
