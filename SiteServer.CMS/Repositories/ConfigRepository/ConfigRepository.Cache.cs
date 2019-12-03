using System;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;

namespace SiteServer.CMS.Repositories
{
    public partial class ConfigRepository
    {
        

        private async Task RemoveCacheAsync()
        {
            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task<Config> GetAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey,
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
