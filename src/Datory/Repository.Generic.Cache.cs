using System.Threading.Tasks;
using CacheManager.Core;
using Datory.Caching;

namespace Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual async Task<ICacheManager<object>> GetCacheManagerAsync()
        {
            return await CachingUtils.GetCacheManagerAsync(Redis);
        }

        public virtual async Task RemoveCacheAsync(params string[] cacheKeys)
        {
            var cacheManager = await CachingUtils.GetCacheManagerAsync(Redis);
            if (cacheKeys == null) return;

            foreach (var cacheKey in cacheKeys)
            {
                cacheManager.Remove(cacheKey);
            }
        }
    }
}
