using System;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public partial class ConfigDao
    {
        private readonly string _cacheKey = CacheManager.Cache.GetKey(nameof(Config));

        private async Task RemoveCacheAsync()
        {
            await CacheManager.Cache.RemoveAsync(_cacheKey);
        }

        public async Task<Config> GetAsync()
        {
            return await CacheManager.Cache.GetOrCreateAsync(_cacheKey,
                async options =>
                {
                    Config info;

                    try
                    {
                        info = await _repository.GetAsync(Q.OrderBy(nameof(Config.Id)));
                    }
                    catch
                    {
                        try
                        {
                            info = await _repository.GetAsync(Q.OrderBy(nameof(Config.Id)));
                        }
                        catch
                        {
                            info = new Config
                            {
                                Id = 0,
                                DatabaseVersion = string.Empty,
                                UpdateDate = DateTime.Now
                            };
                        }
                    }

                    return info;
                });
        }
    }
}
