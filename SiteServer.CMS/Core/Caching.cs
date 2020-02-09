using System;
using System.Threading.Tasks;
using CacheManager.Core;
using Datory.Utils;
using SiteServer.Abstractions;
using StackExchange.Redis;

namespace SiteServer.CMS.Core
{
    public static class Caching
    {
        //public static ICacheManager<object> Cache { get; private set; }

        //public static async Task LoadCacheAsync()
        //{
        //    Cache = await CreateAsync();
        //}

        //private static async Task<ICacheManager<object>> CreateAsync()
        //{
        //    //if (!string.IsNullOrEmpty(WebConfigUtils.Redis))
        //    //{
        //    //    var (isConnectionWorks, _) = await IsConnectionWorksAsync(WebConfigUtils.Redis);
        //    //    if (isConnectionWorks)
        //    //    {
        //    //        var options = Options.Create(new RedisCacheOptions
        //    //        {
        //    //            Configuration = WebConfigUtils.Redis,
        //    //            InstanceName = string.Empty,
        //    //        });
        //    //        return new RedisCache(options);
        //    //    }
        //    //}

        //    //var memoryOptions = Options.Create(new MemoryDistributedCacheOptions());
        //    //return new MemoryDistributedCache(memoryOptions);

        //    if (!string.IsNullOrEmpty(WebConfigUtils.Redis))
        //    {
        //        var (isConnectionWorks, _) = await IsConnectionWorksAsync(WebConfigUtils.Redis);
        //        if (isConnectionWorks)
        //        {
        //            return CacheFactory.Build(settings =>
        //            {
        //                settings
        //                    .WithSystemRuntimeCacheHandle("inProcessCache")
        //                    .WithExpiration(ExpirationMode.None, TimeSpan.Zero)
        //                    .And
        //                    .WithRedisConfiguration("redis", config =>
        //                    {
        //                        //WebConfigUtils.Redis
        //                        config.WithAllowAdmin()
        //                            .WithDatabase(0)
        //                            .WithEndpoint("localhost", 6379);
        //                    })
        //                    .WithMaxRetries(1000)
        //                    .WithRetryTimeout(100)
        //                    .WithJsonSerializer()
        //                    .WithRedisBackplane("redis")
        //                    .WithRedisCacheHandle("redis", true);
        //            });
        //        }
        //    }

        //    return CacheFactory.Build(settings => settings
        //        .WithSystemRuntimeCacheHandle()
        //        .WithExpiration(ExpirationMode.None, TimeSpan.Zero)
        //    );
        //}

        public static string GetAllKey(string tableName)
        {
            return $"ss:{tableName}:all";
        }

        public static string GetAllKey(string tableName, int siteId)
        {
            return $"ss:{tableName}:all:{siteId}";
        }

        public static string GetAllKey(string tableName, string type, string identity)
        {
            return $"ss:{tableName}:all:{type}:{identity}";
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
