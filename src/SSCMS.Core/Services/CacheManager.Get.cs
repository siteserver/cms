using System;
using Microsoft.Extensions.FileProviders;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CacheManager
    {
        public T Get<T>(string key) where T : class
        {
            return _cacheManager.Get(key) as T;
        }

        public string GetByFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return default;

            var cacheKey = CacheUtils.GetPathKey(filePath);
            var value = _cacheManager.Get<string>(cacheKey);

            if (value != null) return value;

            if (FileUtils.IsFileExists(filePath))
            {
                value = FileUtils.ReadText(filePath);
                AddOrUpdateSliding(cacheKey, value, 12 * 60);

                var directoryPath = DirectoryUtils.GetDirectoryPath(filePath);
                var fileName = PathUtils.GetFileName(filePath);
                var fileProvider = new PhysicalFileProvider(directoryPath);

                Action<object> callback = null;
                callback = _ =>
                {
                    // The order here is important. We need to take the token and then apply our changes BEFORE
                    // registering. This prevents us from possible having two change updates to process concurrently.
                    //
                    // If the file changes after we take the token, then we'll process the update immediately upon
                    // registering the callback.
                    var token = fileProvider.Watch(fileName);
                    Remove(cacheKey);
                    token.RegisterChangeCallback(callback, null);
                };

                fileProvider.Watch(fileName).RegisterChangeCallback(callback, null);
            }

            return value;
        }

        public bool Exists(string key)
        {
            return _cacheManager.Exists(key);
        }
    }
}
