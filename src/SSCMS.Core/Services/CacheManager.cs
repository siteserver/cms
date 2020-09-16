using CacheManager.Core;

namespace SSCMS.Core.Services
{
    public partial class CacheManager<TCacheValue> : SSCMS.Services.ICacheManager<TCacheValue>
    {
        private readonly ICacheManager<TCacheValue> _cacheManager;

        public CacheManager(ICacheManager<TCacheValue> cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public IReadOnlyCacheManagerConfiguration Configuration => _cacheManager.Configuration;
    }
}
