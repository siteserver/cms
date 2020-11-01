using CacheManager.Core;

namespace SSCMS.Services
{
    public partial interface ICacheManager
    {
        IReadOnlyCacheManagerConfiguration Configuration { get; }
    }
}