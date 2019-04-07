using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Fx;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Plugins
{
    [RoutePrefix("pages/plugins/install")]
    public class PagesInstallController : ApiController
    {
        private const string RouteConfig = "config";
        private const string RouteDownload = "download";
        private const string RouteUpdate = "update";
        private const string RouteCache = "cache";

        [HttpGet, Route(RouteConfig)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    IsNightly = WebConfigUtils.IsNightlyUpdate,
                    SystemManager.PluginVersion,
                    DownloadPlugins = PluginManager.PackagesIdAndVersionList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteDownload)]
        public IHttpActionResult Download()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                var packageId = Request.GetPostString("packageId");
                var version = Request.GetPostString("version");

                if (!StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSiteServerPlugin))
                {
                    try
                    {
                        PackageUtils.DownloadPackage(packageId, version);
                    }
                    catch
                    {
                        PackageUtils.DownloadPackage(packageId, version);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpdate)]
        public IHttpActionResult Update()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                var packageId = Request.GetPostString("packageId");
                var version = Request.GetPostString("version");
                var packageType = Request.GetPostString("packageType");

                if (!StringUtils.EqualsIgnoreCase(packageId, PackageUtils.PackageIdSiteServerPlugin))
                {
                    string errorMessage;
                    var idWithVersion = $"{packageId}.{version}";
                    if (!PackageUtils.UpdatePackage(idWithVersion, PackageType.Parse(packageType), out errorMessage))
                    {
                        return BadRequest(errorMessage);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        
        [HttpPost, Route(RouteCache)]
        public IHttpActionResult Cache()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasSystemPermissions(ConfigManager.PluginsPermissions.Add))
                {
                    return Unauthorized();
                }

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
