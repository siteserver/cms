using System;
using System.Linq;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Fx;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    [RoutePrefix("pages/plugins/add")]
    public class PagesAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                var dict = PluginManager.GetPluginIdAndVersionDict();
                var list = dict.Keys.ToList();
                var packageIds = TranslateUtils.ObjectCollectionToString(list);

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
