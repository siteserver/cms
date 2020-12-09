using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesAddController
    {
        [HttpPost, Route(RouteProcess)]
        public async Task<ActionResult<CacheUtils.Process>> Process([FromBody] ProcessRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesAdd))
            {
                return Unauthorized();
            }

            var caching = new CacheUtils(_cacheManager);
            return caching.GetProcess(request.Guid);
        }
    }
}