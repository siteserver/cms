using System;
using System.IO;
using CacheManager.Core;
using Datory.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public class CacheUtils
    {
        private readonly ICacheManager<Process> _cacheManager;
        public CacheUtils(ICacheManager<Process> cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public class Process
        {
            public int Total { get; set; }
            public int Current { get; set; }
            public string Message { get; set; }
        }

        public static void SetFileContent(ICacheManager<object> cacheManager, object value, string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var directoryPath = DirectoryUtils.GetDirectoryPath(filePath);
            var fileName = PathUtils.GetFileName(filePath);
            var watcher = new FileSystemWatcher
            {
                Filter = fileName,
                Path = directoryPath,
                EnableRaisingEvents = true
            };

            var cacheKey = GetPathKey(filePath);

            watcher.Changed += (sender, e) =>
            {
                cacheManager.Remove(cacheKey);
            };
            watcher.Renamed += (sender, e) =>
            {
                cacheManager.Remove(cacheKey);
            };
            watcher.Deleted += (sender, e) =>
            {
                cacheManager.Remove(cacheKey);
            };

            var cacheItem = new CacheItem<object>(cacheKey, value, ExpirationMode.Sliding, TimeSpan.FromHours(12));
            cacheManager.AddOrUpdate(cacheItem, _ => value);
        }

        private static string GetProcessCacheKey(string guid)
        {
            return $"ss:{nameof(Process)}:{guid}";
        }

        public void SetProcess(string guid, string message)
        {
            if (string.IsNullOrEmpty(guid)) return;

            //var cache = CacheUtils.Get<Process>(guid);
            var cacheKey = GetProcessCacheKey(guid);
            var process = _cacheManager.Get(cacheKey);
            if (process == null)
            {
                process = new Process
                {
                    Total = 100,
                    Current = 0,
                    Message = message
                };
            }
            else
            {
                process.Total++;
                process.Current++;
                process.Message = message;
            }

            var cacheItem = new CacheItem<Process>(cacheKey, process, ExpirationMode.Sliding, TimeSpan.FromHours(1));

            _cacheManager.AddOrUpdate(cacheItem, _ => process);

            //CacheUtils.InsertHours(guid, cache, 1);
        }

        public Process GetProcess(string guid)
        {
            var cacheKey = GetProcessCacheKey(guid);
            var process = _cacheManager.Get(cacheKey) ?? new Process
            {
                Total = 100,
                Current = 0,
                Message = string.Empty
            };

            return process;
        }

        public static string GetPathKey(string filePath)
        {
            return $"ss:{StringUtils.ToLower(filePath)}";
        }

        public static string GetClassKey(Type type, params string[] values)
        {
            if (values == null || values.Length <= 0) return $"ss:{type.FullName}";
            return $"ss:{type.FullName}:{Utilities.ToString(values, ":")}";
        }

        public static string GetEntityKey(string tableName)
        {
            return $"ss:{tableName}:entity:only";
        }

        public static string GetEntityKey(string tableName, int id)
        {
            return $"ss:{tableName}:entity:{id}";
        }

        public static string GetEntityKey(string tableName, string type, string identity)
        {
            return $"ss:{tableName}:entity:{type}:{identity}";
        }

        public static string GetListKey(string tableName)
        {
            return $"ss:{tableName}:list";
        }

        public static string GetListKey(string tableName, int siteId)
        {
            return $"ss:{tableName}:list:{siteId}";
        }

        public static string GetListKey(string tableName, string type)
        {
            return $"ss:{tableName}:list:{type}";
        }

        public static string GetListKey(string tableName, string type, params string[] identities)
        {
            return $"ss:{tableName}:list:{type}:{Utilities.ToString(identities, ":")}";
        }

        public static string GetCountKey(string tableName, int siteId)
        {
            return $"ss:{tableName}:count:{siteId}";
        }

        public static string GetCountKey(string tableName, int siteId, int channelId)
        {
            return $"ss:{tableName}:count:{siteId}:{channelId}";
        }

        public static string GetCountKey(string tableName, int siteId, int channelId, int adminId)
        {
            return $"ss:{tableName}:count:{siteId}:{channelId}:{adminId}";
        }

        public static string GetCountKey(string tableName, int siteId, int channelId, params string[] identities)
        {
            return $"ss:{tableName}:count:{siteId}:{channelId}:{Utilities.ToString(identities, ":")}";
        }
    }
}
