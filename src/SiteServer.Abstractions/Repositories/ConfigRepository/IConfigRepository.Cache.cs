using System.Threading.Tasks;
using CacheManager.Core;

namespace SiteServer.Abstractions
{
    public partial interface IConfigRepository
    {
        Task ClearAllCache();

        Task<IReadOnlyCacheManagerConfiguration> GetCacheConfigurationAsync();

        Task<Config> GetAsync();
    }
}
