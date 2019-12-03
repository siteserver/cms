using System;
using System.Threading;
using System.Threading.Tasks;
using Datory;
using Microsoft.Extensions.Caching.Distributed;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Caching
{
    public static class DistributedCachingExtensions
    {
        //public static string GetKey(this IDistributedCache distributedCache, string nameofClass, params string[] values)
        //{
        //    var key = $"ss:{nameofClass}";
        //    if (values == null || values.Length <= 0) return key;
        //    return values.Aggregate(key, (current, t) => current + (":" + t));
        //}

        public static string GetEntityKey(this IDistributedCache distributedCache, IRepository repository)
        {
            return $"ss:{repository.TableName}:entity:only";
        }

        public static string GetEntityKey(this IDistributedCache distributedCache, IRepository repository, int id)
        {
            return $"ss:{repository.TableName}:entity:{id}";
        }

        public static string GetEntityKey(this IDistributedCache distributedCache, IRepository repository, string type, string identity)
        {
            return $"ss:{repository.TableName}:entity:{type}:{identity}";
        }

        public static string GetListKey(this IDistributedCache distributedCache, IRepository repository)
        {
            return $"ss:{repository.TableName}:list";
        }

        public static string GetListKey(this IDistributedCache distributedCache, IRepository repository, int siteId)
        {
            return $"ss:{repository.TableName}:list:{siteId}";
        }

        public static string GetListKey(this IDistributedCache distributedCache, IRepository repository, string type)
        {
            return $"ss:{repository.TableName}:list:{type}";
        }

        public static string GetListKey(this IDistributedCache distributedCache, IRepository repository, string type, params string[] identities)
        {
            return $"ss:{repository.TableName}:list:{type}:{StringUtils.Join(identities, ":")}";
        }

        public static string GetCountKey(this IDistributedCache distributedCache, IRepository repository, int siteId)
        {
            return $"ss:{repository.TableName}:count:{siteId}";
        }

        public static string GetCountKey(this IDistributedCache distributedCache, IRepository repository, int siteId, int channelId)
        {
            return $"ss:{repository.TableName}:count:{siteId}:{channelId}";
        }

        public static string GetCountKey(this IDistributedCache distributedCache, IRepository repository, int siteId, int channelId, int adminId)
        {
            return $"ss:{repository.TableName}:count:{siteId}:{channelId}:{adminId}";
        }

        public static string GetCountKey(this IDistributedCache distributedCache, IRepository repository, int siteId, int channelId, params string[] identities)
        {
            return $"ss:{repository.TableName}:count:{siteId}:{channelId}:{StringUtils.Join(identities, ":")}";
        }

        public static async Task<T> GetOrCreateAsync<T>(this IDistributedCache distributedCache, string key, Func<DistributedCacheEntryOptions, Task<T>> factory) where T : class
        {
            var value = await distributedCache.GetAsync<T>(key);
            if (value != null) return value;

            var options = new DistributedCacheEntryOptions();
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

            var options = new DistributedCacheEntryOptions();
            var intValue = await factory(options);
            await distributedCache.SetStringAsync(key, intValue.ToString(), options);

            return intValue;
        }

        public static async Task<string> GetOrCreateStringAsync(this IDistributedCache distributedCache, string key, Func<DistributedCacheEntryOptions, Task<string>> factory)
        {
            var value = await distributedCache.GetStringAsync(key);
            if (value != null) return value;

            var options = new DistributedCacheEntryOptions();
            value = await factory(options);
            await distributedCache.SetStringAsync(key, value.ToString(), options);

            return value;
        }

        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value)
        {
            if (value != null)
            {
                await distributedCache.SetStringAsync(key, TranslateUtils.JsonSerialize(value), new DistributedCacheEntryOptions());
            }
        }

        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            if (value != null)
            {
                await distributedCache.SetStringAsync(key, TranslateUtils.JsonSerialize(value), options, token);
            }
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default) where T : class
        {
            var result = await distributedCache.GetStringAsync(key, token);
            if (result != null)
            {
                return TranslateUtils.JsonDeserialize<T>(result);
            }
            return null;
        }
    }
}