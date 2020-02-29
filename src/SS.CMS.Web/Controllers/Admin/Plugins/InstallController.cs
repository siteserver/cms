using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Packaging;
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

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public InstallController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IPluginManager pluginManager, IDbCacheRepository dbCacheRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _dbCacheRepository = dbCacheRepository;
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
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.PluginVersion,
                DownloadPlugins = _pluginManager.PackagesIdAndVersionList
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
                    PackageUtils.DownloadPackage(_pathManager, request.PackageId, request.Version);
                }
                catch
                {
                    PackageUtils.DownloadPackage(_pathManager, request.PackageId, request.Version);
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
                if (!_pluginManager.UpdatePackage(idWithVersion, TranslateUtils.ToEnum(request.PackageType, PackageType.Library), out var errorMessage))
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
            await _dbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
