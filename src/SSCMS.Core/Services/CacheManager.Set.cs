using System;
using System.IO;
using CacheManager.Core;
using SSCMS.Core.Utils;
using SSCMS.Utils;

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

        public void AddOrUpdateFileWatcher(string filePath, TCacheValue value)
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

            var cacheKey = CacheUtils.GetPathKey(filePath);

            watcher.Changed += (sender, e) =>
            {
                Remove(cacheKey);
            };
            watcher.Renamed += (sender, e) =>
            {
                Remove(cacheKey);
            };
            watcher.Deleted += (sender, e) =>
            {
                Remove(cacheKey);
            };

            AddOrUpdateSliding(cacheKey, value, 12 * 60);
        }
    }
}
