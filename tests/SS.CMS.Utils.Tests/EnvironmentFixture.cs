using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SS.CMS.Core.Services;
using SS.CMS.Services;

namespace SS.CMS.Utils.Tests
{
    public class EnvironmentFixture
    {
        public ISettingsManager SettingsManager { get; }

        public EnvironmentFixture()
        {
            var contentRootPath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("appSettings.json")
                .Build();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheManager = new CacheManager(memoryCache, null);
            SettingsManager = new SettingsManager(config, contentRootPath, PathUtils.Combine(contentRootPath, "wwwroot"));
        }
    }
}
