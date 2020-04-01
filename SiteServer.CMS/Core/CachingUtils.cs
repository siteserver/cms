using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CacheManager.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class Caching
    {
        private static readonly ConcurrentDictionary<string, ICacheManager<object>> CacheManagers = new ConcurrentDictionary<string, ICacheManager<object>>();

        public static ICacheManager<object> CacheManager
        {
            get
            {
                var redis = new Redis(WebConfigUtils.RedisConnectionString);
                string key;
                if (string.IsNullOrEmpty(redis.ConnectionString))
                {
                    key = "Memory";
                }
                else
                {
                    key = "Distributed:" + redis.ConnectionString;
                }
                if (CacheManagers.TryGetValue(key, out var tc))
                {
                    return tc;
                }

                var cacheManager = Create(redis);

                CacheManagers[key] = cacheManager;
                return cacheManager;
            }
        }

        public static bool IsRedis
        {
            get
            {
                var redis = new Redis(WebConfigUtils.RedisConnectionString);
                var key = "Distributed:" + redis.ConnectionString;
                if (CacheManagers.TryGetValue(key, out var _))
                {
                    return true;
                }

                return  false;
            }
        }

        private static ICacheManager<object> Create(IRedis redis)
        {
            if (redis != null && !string.IsNullOrEmpty(redis.ConnectionString))
            {
                var (isConnectionWorks, _) = redis.IsConnectionWorks();
                if (isConnectionWorks)
                {
                    return CacheFactory.Build(settings =>
                    {
                        settings
                            .WithMicrosoftMemoryCacheHandle()
                            .WithExpiration(ExpirationMode.None, TimeSpan.Zero)
                            .And
                            .WithRedisConfiguration("redis", config =>
                            {
                                if (!string.IsNullOrEmpty(redis.Password))
                                {
                                    config.WithPassword(redis.Password);
                                }
                                if (redis.AllowAdmin)
                                {
                                    config.WithAllowAdmin();
                                }

                                config
                                    .WithDatabase(redis.Database)
                                    .WithEndpoint(redis.Host, redis.Port);
                            })
                            .WithMaxRetries(1000)
                            .WithRetryTimeout(100)
                            .WithJsonSerializer()
                            .WithRedisBackplane("redis")
                            .WithRedisCacheHandle("redis", true);
                    });
                }
            }

            return CacheFactory.Build(settings => settings
                .WithMicrosoftMemoryCacheHandle()
                .WithExpiration(ExpirationMode.None, TimeSpan.Zero)
            );
        }

        internal static T GetOrCreate<T>(this ICacheManager<object> cacheManager, string key, Func<T> factory)
        {
            //var obj = CachingManager.Cache.GetOrAdd(key, _ => await factory.Invoke());
            //return Get<T>(obj);
            T value;
            if (cacheManager.Exists(key))
            {
                try
                {
                    value = cacheManager.Get<T>(key);
                    if (value != null) return value;
                }
                catch
                {
                    // ignored
                }
            }

            value = factory.Invoke();
            if (value != null)
            {
                cacheManager.Put(key, value);
            }

            return value;
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
            return $"ss:{tableName}:list:{type}:{TranslateUtils.ObjectCollectionToString(identities, ":")}";
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
    }
}
