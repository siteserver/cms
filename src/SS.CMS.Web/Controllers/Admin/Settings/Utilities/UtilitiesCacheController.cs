using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Utilities
{
    [Route("admin/settings/utilitiesCache")]
    public partial class UtilitiesCacheController : ControllerBase
    {
        private const string Route = "";

        private readonly ICacheManager<object> _cacheManager;
        private readonly IAuthManager _authManager;

        public UtilitiesCacheController(ICacheManager<object> cacheManager, IAuthManager authManager)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }

            var configuration = await DataProvider.ConfigRepository.GetCacheConfigurationAsync();

            return new GetResult
            {
                Configuration = configuration
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> ClearCache()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUtilitiesCache))
            {
                return Unauthorized();
            }

            await DataProvider.ConfigRepository.ClearAllCache();
            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();
            _cacheManager.Clear();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
