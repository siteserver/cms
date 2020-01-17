using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using SiteServer.Abstractions;
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
            return $"ss:{tableName}:list:{type}:{StringUtils.Join(identities, ":")}";
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
            return $"ss:{tableName}:count:{siteId}:{channelId}:{StringUtils.Join(identities, ":")}";
        }
    }
}
