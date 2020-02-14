using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    [RoutePrefix("pages/settings/utilityCache")]
    public partial class PagesUtilityCacheController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilityCache))
            {
                return Request.Unauthorized<GetResult>();
            }

            var configuration = await DataProvider.ConfigRepository.GetCacheConfigurationAsync();

            return new GetResult
            {
                Configuration = configuration
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> ClearCache()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilityCache))
            {
                return Request.Unauthorized<BoolResult>();
            }

            await DataProvider.ConfigRepository.ClearAllCache();

            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
