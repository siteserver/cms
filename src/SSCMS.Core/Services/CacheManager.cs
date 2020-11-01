using CacheManager.Core;

namespace SSCMS.Core.Services
{
    public partial class CacheManager : SSCMS.Services.ICacheManager
    {
        private readonly ICacheManager<object> _cacheManager;

        public CacheManager(ICacheManager<object> cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public IReadOnlyCacheManagerConfiguration Configuration => _cacheManager.Configuration;
    }
}
