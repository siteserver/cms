using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    
    [RoutePrefix("pages/plugins/view")]
    public class PagesViewController : ApiController
    {
        private const string Route = "{pluginId}";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get(string pluginId)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }
                
                var plugin = await PluginManager.GetPluginAsync(pluginId);

                return Ok(new
                {
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    SystemManager.PluginVersion,
                    Installed = plugin != null,
                    InstalledVersion = plugin != null ? plugin.Version : string.Empty,
                    Package = plugin
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
