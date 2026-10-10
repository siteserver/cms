using System.Collections.Generic;
using CacheManager.Core;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Tests.Utils
{
    /// <summary>
    /// 内存版 ICacheManager，仅用于单元测试（不处理过期，过期逻辑由被测代码自身保存的时间戳负责）。
    /// </summary>
    public class FakeCacheManager : ICacheManager
    {
        private readonly Dictionary<string, object> _items = new Dictionary<string, object>();

        public IReadOnlyCacheManagerConfiguration Configuration => null;

        public T Get<T>(string key) => _items.TryGetValue(key, out var value) ? TranslateUtils.Get<T>(value) : default;

        public string GetByFilePath(string filePath) => string.Empty;

        public bool Exists(string key) => _items.ContainsKey(key);

        public void Remove(string key) => _items.Remove(key);

        public void Clear() => _items.Clear();

        public void AddOrUpdateSliding<T>(string key, T value, int minutes) => _items[key] = value;

        public void AddOrUpdateAbsolute<T>(string key, T value, int minutes) => _items[key] = value;

        public void AddOrUpdate<T>(string key, T value) => _items[key] = value;

        public void Set(string key, object value) => _items[key] = value;
    }
}
