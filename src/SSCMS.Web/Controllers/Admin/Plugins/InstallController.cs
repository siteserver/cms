using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class InstallController : ControllerBase
    {
        private const string Route = "plugins/install";
        private const string RouteActionsDownload = "plugins/install/actions/download";
        private const string RouteActionsUpdate = "plugins/install/actions/update";
        private const string RouteActionsRestart = "plugins/install/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;

        public InstallController(IHostApplicationLifetime hostApplicationLifetime, ISettingsManager settingsManager,
            IAuthManager authManager, IPluginManager pluginManager)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.Version
            };
        }

        [HttpPost, Route(RouteActionsDownload)]
        public async Task<ActionResult<BoolResult>> Download([FromBody]DownloadRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            _pluginManager.Install(request.PluginId, request.Version);

            await _authManager.AddAdminLogAsync("安装插件", $"插件:{request.PluginId}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsUpdate)]
        public async Task<ActionResult<BoolResult>> Update([FromBody]UploadRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var idWithVersion = $"{request.PluginId}.{request.Version}";
            //if (!_pluginManager.UpdatePackage(idWithVersion, TranslateUtils.ToEnum(request.PackageType, PackageType.Library), out var errorMessage))
            //{
            //    return this.Error(errorMessage);
            //}

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsRestart)]
        public async Task<ActionResult<BoolResult>> Restart()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            
            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
