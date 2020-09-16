using CacheManager.Core;

namespace SSCMS.Services
{
    public partial interface ICacheManager<TCacheValue>
    {
        IReadOnlyCacheManagerConfiguration Configuration { get; }
    }
}