using System;
using System.Linq;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    [OpenApiIgnore]
    [RoutePrefix("pages/plugins/add")]
    public class PagesAddController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
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
