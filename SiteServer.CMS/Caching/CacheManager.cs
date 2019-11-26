using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using SiteServer.Utils;
using StackExchange.Redis;

namespace SiteServer.CMS.Caching
{
    public static class CacheManager
    {
        public static IDistributedCache Cache { get; private set; }

        public static async Task LoadCacheAsync()
        {
            Cache = await CreateAsync();
        }

        private static async Task<IDistributedCache> CreateAsync()
        {
            if (!string.IsNullOrEmpty(WebConfigUtils.Redis))
            {
                var (isConnectionWorks, _) = await IsConnectionWorksAsync(WebConfigUtils.Redis);
                if (isConnectionWorks)
                {
                    var options = Options.Create(new RedisCacheOptions
                    {
                        Configuration = WebConfigUtils.Redis,
                        InstanceName = string.Empty,
                    });
                    return new RedisCache(options);
                }
            }

            var memoryOptions = Options.Create(new MemoryDistributedCacheOptions());
            return new MemoryDistributedCache(memoryOptions);
        }

        private static async Task<(bool IsConnectionWorks, string ErrorMessage)> IsConnectionWorksAsync(string redisConnectionString)
        {
            var isSuccess = false;
            var errorMessage = string.Empty;
            try
            {
                using var connection = ConnectionMultiplexer.Connect(redisConnectionString);
                var cache = connection.GetDatabase();

                var cacheCommand = "PING";
                var result = await cache.ExecuteAsync(cacheCommand);

                isSuccess = result != null && result.ToString() == "PONG";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return (isSuccess, errorMessage);
        }
    }
}
