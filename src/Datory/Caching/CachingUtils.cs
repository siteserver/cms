using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CacheManager.Core;

namespace Datory.Caching
{
    public static class CachingUtils
    {
        private static readonly ConcurrentDictionary<string, ICacheManager<object>> CacheManagers = new ConcurrentDictionary<string, ICacheManager<object>>();

        public static async Task<ICacheManager<object>> GetCacheManagerAsync(IRedis redis)
        {
            string key;
            if (redis == null || string.IsNullOrEmpty(redis.ConnectionString))
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

            var cacheManager = await CreateAsync(redis);

            CacheManagers[key] = cacheManager;
            return cacheManager;
        }

        private static async Task<ICacheManager<object>> CreateAsync(IRedis redis)
        {
            if (redis != null && !string.IsNullOrEmpty(redis.ConnectionString))
            {
                var (isConnectionWorks, _) = await redis.IsConnectionWorksAsync();
                if (isConnectionWorks)
                {
                    return CacheFactory.Build(settings =>
                    {
                        settings
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

        internal static async Task<T> GetOrCreateAsync<T>(this ICacheManager<object> cacheManager, string key, Func<Task<T>> factory)
        {
            T value = default;
            if (cacheManager.Exists(key))
            {
                try
                {
                    value = cacheManager.Get<T>(key);
                }
                catch
                {
                    // ignored
                }
            }

            if (value != null) return value;

            value = await factory.Invoke();
            if (value != null)
            {
                cacheManager.Put(key, value);
            }

            return value;
        }
    }
    //public static class CachingUtils
    //{
    //    public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache distributedCache, string key, Func<Task<T>> factory, DistributedCacheEntryOptions options = null)
    //    {
    //        var value = await distributedCache.GetAsync<T>(key);
    //        if (value != null) return value;

    //        value = await factory.Invoke();
    //        await SetAsync(distributedCache, key, value, options);

    //        return value;
    //    }

    //    public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options = null)
    //    {
    //        if (value != null)
    //        {
    //            if (options == null)
    //            {
    //                options = new DistributedCacheEntryOptions();
    //            }

    //            if (IsSimple(typeof(T)))
    //            {
    //                var data = Encoding.UTF8.GetBytes(value.ToString());
    //                await distributedCache.SetAsync(key, data, options);
    //            }
    //            else
    //            {
    //                var data = Encoding.UTF8.GetBytes(Utilities.JsonSerialize(value));
    //                await distributedCache.SetAsync(key, data, options);
    //            }
    //        }
    //    }

    //    public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key)
    //    {
    //        var result = await distributedCache.GetAsync(key);
    //        if (result == null) return default;

    //        if (IsSimple(typeof(T)))
    //        {
    //            var data = Encoding.UTF8.GetString(result);
    //            return Get<T>(data);
    //        }
    //        else
    //        {
    //            var data = Encoding.UTF8.GetString(result);
    //            return Utilities.JsonDeserialize<T>(data);

    //        }
    //    }

    //    private static T Get<T>(object value, T defaultValue = default(T))
    //    {
    //        switch (value)
    //        {
    //            case null:
    //                return defaultValue;
    //            case T variable:
    //                return variable;
    //            default:
    //                try
    //                {
    //                    return (T)Convert.ChangeType(value, typeof(T));
    //                }
    //                catch (InvalidCastException)
    //                {
    //                    return defaultValue;
    //                }
    //        }
    //    }

    //    private static bool IsSimple(Type type)
    //    {
    //        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
    //        {
    //            // nullable type, check if the nested type is simple.
    //            return IsSimple(type.GetGenericArguments()[0]);
    //        }
    //        return type.IsPrimitive
    //               || type.IsEnum
    //               || type == typeof(string)
    //               || type == typeof(decimal);
    //    }
    //}
}
