using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ManageController : ControllerBase
    {
        private const string Route = "plugins/manage";
        private const string RouteActionsDisable = "plugins/manage/actions/disable";
        private const string RouteActionsDelete = "plugins/manage/actions/delete";
        private const string RouteActionsRestart = "plugins/manage/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;

        public ManageController(IHostApplicationLifetime hostApplicationLifetime, ISettingsManager settingsManager, IAuthManager authManager, IPluginManager pluginManager)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            //var dict = await _pluginManager.GetPluginIdAndVersionDictAsync();
            //var list = dict.Keys.ToList();
            //var packageIds = Utilities.ToString(list);

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                Version = _settingsManager.Version,
                AllPlugins = _pluginManager.Plugins
            };
        }

        [HttpPost, Route(RouteActionsRestart)]
        public async Task<ActionResult<BoolResult>> Restart()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsDisable)]
        public async Task<ActionResult<BoolResult>> Disable([FromBody] DisableRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            await _pluginManager.DisableAsync(request.PluginId, request.Disabled);

            await _authManager.AddAdminLogAsync(request.Disabled ? "禁用插件" : "启用插件", $"插件:{request.PluginId}");

            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(request.PluginId))
            {
                return this.Error("参数不正确");
            }

            _pluginManager.UnInstall(request.PluginId);

            await _authManager.AddAdminLogAsync("卸载插件", $"插件:{request.PluginId}");

            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
