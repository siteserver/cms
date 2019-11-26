using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.Caching.Distributed;
using SiteServer.Utils;

namespace SiteServer.CMS.Caching
{
    public static class DistributedCachingExtensions
    {
        public static string GetKey(this IDistributedCache distributedCache, string nameofClass, params string[] values)
        {
            var key = $"ss:{nameofClass}";
            if (values == null || values.Length <= 0) return key;
            return values.Aggregate(key, (current, t) => current + (":" + t));
        }

        public static string GetEntityKey(this IDistributedCache distributedCache, IRepository repository, int id)
        {
            return $"ss:{repository.TableName}:entity:{id}";
        }

        public static string GetListKey(this IDistributedCache distributedCache, IRepository repository)
        {
            return $"ss:{repository.TableName}:list";
        }

        public static string GetListKey(this IDistributedCache distributedCache, IRepository repository, string type)
        {
            return $"ss:{repository.TableName}:list:{type}";
        }

        public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache distributedCache, string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class
        {
            var value = await distributedCache.GetAsync<T>(key);
            if (value != null) return value;

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2));
            value = await factory(options);
            if (value != null)
            {
                await distributedCache.SetAsync<T>(key, value, options);
            }

            return value;
        }

        public static async Task<int> GetOrCreateIntAsync(this IDistributedCache distributedCache, string key, Func<DistributedCacheEntryOptions, Task<int>> factory)
        {
            var value = await distributedCache.GetStringAsync(key);
            if (value != null) return TranslateUtils.ToInt(value);

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2));
            var intValue = await factory(options);
            await distributedCache.SetStringAsync(key, intValue.ToString(), options);

            return intValue;
        }

        public static async Task<string> GetOrCreateStringAsync(this IDistributedCache distributedCache, string key, Func<DistributedCacheEntryOptions, Task<string>> factory)
        {
            var value = await distributedCache.GetStringAsync(key);
            if (value != null) return value;

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2));
            value = await factory(options);
            await distributedCache.SetStringAsync(key, value.ToString(), options);

            return value;
        }

        private static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value)
        {
            if (value != null)
            {
                await distributedCache.SetAsync(key, TranslateUtils.BinarySerialize(value), new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2)), default(CancellationToken));
            }
        }

        private static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            if (value != null)
            {
                await distributedCache.SetAsync(key, TranslateUtils.BinarySerialize(value), options, token);
            }
        }

        private static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var result = await distributedCache.GetAsync(key, token);
            if (result != null)
            {
                return TranslateUtils.BinaryDeserialize<T>(result);
            }
            return null;
        }
    }
}