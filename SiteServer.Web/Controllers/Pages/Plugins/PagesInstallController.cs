using System;
using System.Web.Http;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    [RoutePrefix("pages/plugins/install")]
    public class PagesInstallController : ApiController
    {
        private const string RouteConfig = "config";

        [HttpGet, Route(RouteConfig)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    Version = SystemManager.PluginVersion,
                    DownloadPlugins = PluginManager.PackagesIdAndVersionList,
                    DownloadApiUrl = ApiRouteDownload.GetUrl(ApiManager.InnerApiUrl),
                    UpdateApiUrl = ApiRouteUpdate.GetUrl(ApiManager.InnerApiUrl),
                    ClearCacheApiUrl = ApiRouteClearCache.GetUrl(ApiManager.InnerApiUrl)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
