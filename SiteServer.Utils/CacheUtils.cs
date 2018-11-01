using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace SiteServer.Utils
{
    public static class CacheUtils
    {
        private static readonly Cache Cache;

        /// <summary>
        /// Static initializer should ensure we only have to look up the current cache
        /// instance once.
        /// </summary>
        static CacheUtils()
        {
            var context = HttpContext.Current;
            Cache = context != null ? context.Cache : HttpRuntime.Cache;
        }

        public static void ClearAll()
        {
            if (Cache == null) return;

            var cacheEnum = Cache.GetEnumerator();
            var keys = new List<string>();
            while (cacheEnum.MoveNext())
            {
                if (cacheEnum.Key != null) keys.Add(cacheEnum.Key.ToString());
            }

            foreach (var key in keys)
            {
                Cache.Remove(key);
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
            if (Cache == null) return;

            var cacheEnum = Cache.GetEnumerator();
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (cacheEnum.MoveNext())
            {
                if (cacheEnum.Key != null && regex.IsMatch(cacheEnum.Key.ToString()))
                {
                    Cache.Remove(cacheEnum.Key.ToString());
                }
            }
        }

        /// <summary>
        /// Removes the specified key from the cache
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            Cache?.Remove(key);
        }

        public static void Insert(string key, object obj)
        {
            InnerInsert(key, obj, null, Cache.NoSlidingExpiration);
        }

        public static void Insert(string key, object obj, string filePath)
        {
            InnerInsert(key, obj, filePath, Cache.NoSlidingExpiration);
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

        public static void InsertSeconds(string key, object obj, int seconds)
        {
            InnerInsert(key, obj, null, TimeSpan.FromSeconds(seconds));
        }

        private static void InnerInsert(string key, object obj, string filePath, TimeSpan timeSpan)
        {
            if (Cache == null) return;

            if (string.IsNullOrEmpty(key)) return;

            Cache.Remove(key);
            if (obj != null)
            {
                Cache.Insert(key, obj, string.IsNullOrEmpty(filePath) ? null : new CacheDependency(filePath), Cache.NoAbsoluteExpiration, timeSpan, CacheItemPriority.Normal, null);
            }
        }

        public static object Get(string key)
        {
            return Cache?.Get(key);
        }

        public static int GetInt(string key, int notFound)
        {
            var retval = Get(key);
            if (retval == null)
            {
                return notFound;
            }
            return (int) retval;
        }

        public static DateTime GetDateTime(string key, DateTime notFound)
        {
            var retval = Get(key);
            if (retval == null)
            {
                return notFound;
            }
            return (DateTime)retval;
        }

        public static T Get<T>(string key) where T : class
        {
            return Cache?.Get(key) as T;
        }

        public static bool Exists(string key)
        {
            var val = Cache?.Get(key);
            return val != null;
        }

        public static bool Exists<T>(string key) where T : class
        {
            var val = Cache?.Get(key) as T;
            return val != null;
        }

        public static List<string> AllKeys
        {
            get
            {
                var keys = new List<string>();

                if (Cache == null) return keys;

                var cacheEnum = Cache.GetEnumerator();
                while (cacheEnum.MoveNext())
                {
                    if (cacheEnum.Key != null) keys.Add(cacheEnum.Key.ToString());
                }

                return keys;
            }
        }

        public static int Count => Cache?.Count ?? 0;
    }
}
