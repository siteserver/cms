using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace BaiRong.Core
{
    public class CacheUtils
    {
        private CacheUtils() { }

        public static readonly int DayFactor = 43200;
        public static readonly int HourFactor = 3600;
        public static readonly int MinuteFactor = 60;

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

        /// <summary>
        /// Removes all items from the Cache
        /// </summary>
        public static void Clear()
        {
            var cacheEnum = Cache.GetEnumerator();
            var al = new ArrayList();
            while (cacheEnum.MoveNext())
            {
                al.Add(cacheEnum.Key);
            }

            foreach (string key in al)
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

        public static void RemoveByPattern(string pattern)
        {
            var cacheEnum = Cache.GetEnumerator();
            var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (cacheEnum.MoveNext())
            {
                if (regex.IsMatch(cacheEnum.Key.ToString()))
                    Cache.Remove(cacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// Removes the specified key from the cache
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Insert the current "obj" into the cache. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Insert(string key, object obj)
        {
            Insert(key, obj, null, 1);
        }

        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, dep, HourFactor * 12);
        }

        public static void Insert(string key, object obj, int seconds)
        {
            Insert(key, obj, null, seconds);
        }

        public static void Insert(string key, object obj, int seconds, CacheItemPriority priority)
        {
            Insert(key, obj, null, seconds, priority);
        }

        public static void Insert(string key, object obj, CacheDependency dep, int seconds)
        {
            Insert(key, obj, dep, seconds, CacheItemPriority.Normal);
        }

        public static void Insert(string key, object obj, CacheDependency dep, int seconds, CacheItemPriority priority)
        {
            if (obj != null)
            {
                Cache.Add(key, obj, dep, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, priority, null);
            }

        }

        public static void MicroInsert(string key, object obj, int secondFactor)
        {
            if (obj != null)
            {
                Cache.Insert(key, obj, null, DateTime.Now.AddSeconds(secondFactor), TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Insert an item into the cache for the Maximum allowed time
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Max(string key, object obj)
        {
            Max(key, obj, null);
        }

        public static void Max(string key, object obj, CacheDependency dep)
        {
            if (obj != null)
            {
                Cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
            }
        }

        public static object Get(string key)
        {
            return Cache[key];
        }

        public static int GetInt(string key, int notFound)
        {
            var retval = Get(key);
            return (int?) retval ?? notFound;
        }

        public static DateTime GetDateTime(string key, DateTime notFound)
        {
            var retval = Get(key);
            return (DateTime?)retval ?? notFound;
        }

        public static T Get<T>(string key) where T : class
        {
            return Cache[key] as T;
        }

        public static string GetCacheKeyByUserName(string key, string userName)
        {
            return $"{key}_BY_USER_{userName}";
        }

        /// <summary>
        /// Return Cache Count
        /// </summary>
        /// <returns></returns>
        public static int GetCacheCount()
        {
            return Cache.Count;
        }

        /// <summary>
        /// Return Cache Size (M)
        /// </summary>
        /// <returns></returns>
        public static long GetCacheEnabledPercent()
        {
            return Cache.EffectivePercentagePhysicalMemoryLimit;
        }
    }
}
