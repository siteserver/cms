using System.IO;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CacheManager
    {
        public T Get<T>(string key)
        {
            return TranslateUtils.Get<T>(_cacheManager.Get(key));
        }

        public string GetByFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return string.Empty;

            var cacheKey = CacheUtils.GetPathKey(filePath);
            var value = _cacheManager.Get<string>(cacheKey);

            if (value != null) return value;

            value = FileUtils.ReadText(filePath);
            AddOrUpdateSliding(cacheKey, value, 12 * 60);

            return value;
        }

        public bool Exists(string key)
        {
            return _cacheManager.Exists(key);
        }
    }
}
