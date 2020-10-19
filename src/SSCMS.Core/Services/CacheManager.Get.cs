using System;
using System.Collections.Generic;
using System.IO;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CacheManager<TCacheValue>
    {
        private static readonly Dictionary<string, FileSystemWatcher> _fileDependencies =
            new Dictionary<string, FileSystemWatcher>(StringComparer.OrdinalIgnoreCase);

        public TCacheValue Get(string key)
        {
            return _cacheManager.Get(key);
        }

        public TCacheValue GetByFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return default;

            var cacheKey = CacheUtils.GetPathKey(filePath);
            var value = _cacheManager.Get(cacheKey);

            if (!_fileDependencies.ContainsKey(cacheKey))
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

                _fileDependencies[cacheKey] = watcher;
            }

            return value;
        }

        public bool Exists(string key)
        {
            return _cacheManager.Exists(key);
        }
    }
}
