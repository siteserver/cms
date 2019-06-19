using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace SS.CMS.Core.Services
{
    public partial class CacheManager
    {
        public void ClearAll()
        {
            var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }
        }

        public void RemoveByStartString(string startString)
        {
            if (!string.IsNullOrEmpty(startString))
            {
                RemoveByPattern(startString + "([w+]*)");
            }
        }

        private void RemoveByPattern(string pattern)
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

        public void Insert(string key, object obj)
        {
            InnerInsert(key, obj, null, TimeSpan.FromDays(365));
        }

        public void Insert(string key, object obj, string filePath)
        {
            InnerInsert(key, obj, filePath, TimeSpan.FromDays(365));
        }

        public void Insert(string key, object obj, TimeSpan timeSpan, string filePath)
        {
            InnerInsert(key, obj, filePath, timeSpan);
        }

        public void InsertHours(string key, object obj, int hours)
        {
            InnerInsert(key, obj, null, TimeSpan.FromHours(hours));
        }

        public void InsertMinutes(string key, object obj, int minutes)
        {
            InnerInsert(key, obj, null, TimeSpan.FromMinutes(minutes));
        }

        private void InnerInsert(string key, object obj, string filePath, TimeSpan timeSpan)
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

        public object Get(string key)
        {
            return MemoryCache.Default.Get(key);
        }

        public bool Exists(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        public List<string> AllKeys
        {
            get
            {
                return MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            }
        }
    }
}
