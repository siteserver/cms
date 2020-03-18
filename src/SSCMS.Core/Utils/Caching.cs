using Datory.Utils;

namespace SSCMS.Core.Utils
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

        public class Process
        {
            public int Total { get; set; }
            public int Current { get; set; }
            public string Message { get; set; }
        }

        public static void SetProcess(string guid, string message)
        {
            if (string.IsNullOrEmpty(guid)) return;

            var cache = CacheUtils.Get<Process>(guid);
            if (cache == null)
            {
                cache = new Process
                {
                    Total = 100,
                    Current = 0,
                    Message = message
                };
            }
            else
            {
                cache.Total++;
                cache.Current++;
                cache.Message = message;
            }
            CacheUtils.InsertHours(guid, cache, 1);
        }

        public static Process GetProcess(string guid)
        {
            var cache = CacheUtils.Get<Process>(guid) ?? new Process
            {
                Total = 100,
                Current = 0,
                Message = string.Empty
            };

            return cache;
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
