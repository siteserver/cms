using CacheManager.Core;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesCacheController
    {
        public class GetResult
        {
            public IReadOnlyCacheManagerConfiguration Configuration { get; set; }
        }
    }
}