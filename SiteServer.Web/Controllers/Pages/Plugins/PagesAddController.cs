using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    
    [RoutePrefix("pages/plugins/add")]
    public class PagesAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
                {
                    return Unauthorized();
                }

                var dict = await PluginManager.GetPluginIdAndVersionDictAsync();
                var list = dict.Keys.ToList();
                var packageIds = Utilities.ToString(list);

                return Ok(new
                {
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    SystemManager.PluginVersion,
                    PackageIds = packageIds
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
