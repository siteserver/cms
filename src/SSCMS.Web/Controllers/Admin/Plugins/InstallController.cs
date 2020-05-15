using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Core.Packaging;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class InstallController : ControllerBase
    {
        private const string RouteConfig = "plugins/install/config";
        private const string RouteDownload = "plugins/install/download";
        private const string RouteUpdate = "plugins/install/update";
        private const string RouteCache = "plugins/install/cache";

        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public InstallController(ICacheManager<object> cacheManager, ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IOldPluginManager pluginManager, IDbCacheRepository dbCacheRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _dbCacheRepository = dbCacheRepository;
        }

        [HttpGet, Route(RouteConfig)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.Version,
                DownloadPlugins = _pluginManager.PackagesIdAndVersionList
            };
        }

        [HttpPost, Route(RouteDownload)]
        public async Task<ActionResult<BoolResult>> Download([FromBody]DownloadRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            CloudUtils.Dl.DownloadPlugin(_pathManager, request.PackageId, request.Version);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpdate)]
        public async Task<ActionResult<BoolResult>> Update([FromBody]UploadRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var idWithVersion = $"{request.PackageId}.{request.Version}";
            if (!_pluginManager.UpdatePackage(idWithVersion, TranslateUtils.ToEnum(request.PackageType, PackageType.Library), out var errorMessage))
            {
                return this.Error(errorMessage);
            }

            return new BoolResult
            {
                Value = true
            };
        }

        
        [HttpPost, Route(RouteCache)]
        public async Task<ActionResult<BoolResult>> Cache()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            _cacheManager.Clear();
            await _dbCacheRepository.ClearAsync();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
