using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Packaging;
using SS.CMS.Plugins;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/install")]
    public partial class InstallController : ControllerBase
    {
        private const string RouteConfig = "config";
        private const string RouteDownload = "download";
        private const string RouteUpdate = "update";
        private const string RouteCache = "cache";

        private readonly IAuthManager _authManager;

        public InstallController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(RouteConfig)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                IsNightly = WebConfigUtils.IsNightlyUpdate,
                PluginVersion = SystemManager.PluginVersion,
                DownloadPlugins = PluginManager.PackagesIdAndVersionList
            };
        }

        [HttpPost, Route(RouteDownload)]
        public async Task<ActionResult<BoolResult>> Download([FromBody]DownloadRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            if (!StringUtils.EqualsIgnoreCase(request.PackageId, PackageUtils.PackageIdSiteServerPlugin))
            {
                try
                {
                    PackageUtils.DownloadPackage(request.PackageId, request.Version);
                }
                catch
                {
                    PackageUtils.DownloadPackage(request.PackageId, request.Version);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<BoolResult>> Update([FromBody]UploadRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            if (!StringUtils.EqualsIgnoreCase(request.PackageId, PackageUtils.PackageIdSiteServerPlugin))
            {
                var idWithVersion = $"{request.PackageId}.{request.Version}";
                if (!PackageUtils.UpdatePackage(idWithVersion, TranslateUtils.ToEnum(request.PackageType, PackageType.Library), out var errorMessage))
                {
                    return this.Error(errorMessage);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }

        
        [HttpPost, Route(RouteCache)]
        public async Task<ActionResult<BoolResult>> Cache()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();
            await DataProvider.DbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
