using System;
using System.IO;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public class CacheUtils
    {
        private readonly ICacheManager _cacheManager;
        public CacheUtils(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public class Process
        {
            public int Total { get; set; }
            public int Current { get; set; }
            public string Message { get; set; }
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
            var process = _cacheManager.Get<Process>(cacheKey);
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

            _cacheManager.AddOrUpdateSliding(cacheKey, process, 60);

            //CacheUtils.InsertHours(guid, cache, 1);
        }

        public Process GetProcess(string guid)
        {
            var cacheKey = GetProcessCacheKey(guid);
            var process = _cacheManager.Get<Process>(cacheKey) ?? new Process
            {
                Total = 100,
                Current = 0,
                Message = string.Empty
            };

            return process;
        }

        public static string GetPathKey(string filePath)
        {
            return $"ss:{StringUtils.ToLower(filePath)}:{DateUtils.GetUnixTimestamp(File.GetLastWriteTime(filePath))}";
        }

        public static string GetClassKey(Type type, params string[] values)
        {
            if (values == null || values.Length <= 0) return $"ss:{type.FullName}";
            return $"ss:{type.FullName}:{ListUtils.ToString(values, ":")}";
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
            return $"ss:{tableName}:list:{type}:{ListUtils.ToString(identities, ":")}";
        }
    }
}
