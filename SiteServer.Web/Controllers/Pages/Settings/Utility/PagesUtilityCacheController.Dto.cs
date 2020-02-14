using CacheManager.Core;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    public partial class PagesUtilityCacheController
    {
        public class GetResult
        {
            public IReadOnlyCacheManagerConfiguration Configuration { get; set; }
        }
    }
}