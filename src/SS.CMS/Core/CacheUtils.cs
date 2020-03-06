using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using SS.CMS.Abstractions;

namespace SS.CMS.Core
{
    public static class CacheUtils
    {
        private const string CachePrefix = "CacheUtils";
        private static IMemoryCache _cache;

        static CacheUtils()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public static void ClearAll()
        {
            _cache.Dispose();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        public static void Insert(string key, object obj)
        {
            InnerInsert(key, obj, TimeSpan.Zero);
        }

        public static void InsertHours(string key, object obj, int hours)
        {
            InnerInsert(key, obj, TimeSpan.FromHours(hours));
        }

        public static void InsertMinutes(string key, object obj, int minutes)
        {
            InnerInsert(key, obj, TimeSpan.FromMinutes(minutes));
        }

        private static void InnerInsert(string key, object obj, TimeSpan timeSpan)
        {
            if (_cache == null) return;

            if (string.IsNullOrEmpty(key) || obj == null) return;

            if (timeSpan == TimeSpan.Zero)
            {
                _cache.Set(key, obj);
            }
            else
            {
                _cache.Set(key, obj, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = timeSpan
                });
            }
        }

        public static void Insert(string key, object obj, string filePath)
        {
            if (_cache == null) return;

            if (!string.IsNullOrEmpty(filePath))
            {
                var directoryPath = DirectoryUtils.GetDirectoryPath(filePath);
                var fileName = PathUtils.GetFileName(filePath);
                var watcher = new FileSystemWatcher
                {
                    Filter = fileName,
                    Path = directoryPath,
                    EnableRaisingEvents = true
                };
                watcher.Changed += (sender, e) =>
                {
                    _cache.Remove(key);
                };
                watcher.Renamed += (sender, e) =>
                {
                    _cache.Remove(key);
                };
                watcher.Deleted += (sender, e) =>
                {
                    _cache.Remove(key);
                };
                _cache.Set(key, obj, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(12)
                });
            }
        }

        public static T Get<T>(string key) where T : class
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        public static bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
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