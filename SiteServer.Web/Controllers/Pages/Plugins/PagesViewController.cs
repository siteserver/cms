using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    [RoutePrefix("pages/plugins/view")]
    public class PagesViewController : ApiController
    {
        private const string Route = "{pluginId}";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get(string pluginId)
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }
                
                var plugin = PluginManager.GetPlugin(pluginId);

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
