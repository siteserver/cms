using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace SS.CMS.Utils
{
    public static class CacheUtils
    {
        public static void ClearAll()
        {
            var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }
        }

        public static void RemoveByStartString(string startString)
        {
            if (!string.IsNullOrEmpty(startString))
            {
                RemoveByPattern(startString + "([w+]*)");
            }
        }

        private static void RemoveByPattern(string pattern)
        {
            var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            foreach (var cacheKey in cacheKeys)
            {
                if (cacheKey != null && regex.IsMatch(cacheKey))
                {
                    MemoryCache.Default.Remove(cacheKey);
                }
            }
        }

        public static void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public static void Insert(string key, object obj)
        {
            InnerInsert(key, obj, null, TimeSpan.FromDays(365));
        }

        public static void Insert(string key, object obj, string filePath)
        {
            InnerInsert(key, obj, filePath, TimeSpan.FromDays(365));
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
            if (string.IsNullOrEmpty(key)) return;

            Remove(key);
            if (obj == null) return;

            var policy = new CacheItemPolicy
            {
                SlidingExpiration = timeSpan
            };

            if (!string.IsNullOrEmpty(filePath))
            {
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string> { filePath }));
            }

            MemoryCache.Default.Set(key, obj, policy);
        }

        public static object Get(string key)
        {
            return MemoryCache.Default.Get(key);
        }

        public static T Get<T>(string key)
        {
            if (MemoryCache.Default[key] is T t)
            {
                return t;
            }
            return default;
        }

        public static bool Exists(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        public static List<string> AllKeys
        {
            get
            {
                return MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            }
        }
    }
}
