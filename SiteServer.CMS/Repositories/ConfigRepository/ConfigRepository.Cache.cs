using System;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using Datory.Caching;

namespace SiteServer.CMS.Repositories
{
    public partial class ConfigRepository
    {
        public async Task ClearAllCache()
        {
            var cacheManager = await _repository.GetCacheManagerAsync();
            cacheManager.Clear();
        }

        public async Task<Config> GetAsync()
        {
            try
            {
                return await _repository.GetAsync(Q
                    .OrderBy(nameof(Config.Id))
                    .CachingGet(_cacheKey)
                );
            }
            catch
            {
                return new Config
                {
                    Id = 0,
                    DatabaseVersion = string.Empty,
                    UpdateDate = DateTime.Now
                };
            }
        }
    }
}
