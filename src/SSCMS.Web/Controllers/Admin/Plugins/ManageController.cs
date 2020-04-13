using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Datory.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Core.Packaging;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = Constants.RoleTypeAdministrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ManageController : ControllerBase
    {
        private const string Route = "plugins/manage";
        private const string RoutePluginId = "plugins/manage/{pluginId}";
        private const string RouteActionsReload = "plugins/manage/actions/reload";
        private const string RoutePluginIdEnable = "plugins/manage/{pluginId}/actions/enable";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IOldPluginManager _oldPluginManager;
        private readonly IPluginRepository _pluginRepository;
        private readonly IPluginManager _pluginManager;

        public ManageController(IHostApplicationLifetime hostApplicationLifetime, ICacheManager<object> cacheManager, ISettingsManager settingsManager, IAuthManager authManager, IOldPluginManager oldPluginManager, IPluginRepository pluginRepository, IPluginManager pluginManager)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _oldPluginManager = oldPluginManager;
            _pluginRepository = pluginRepository;
            _pluginManager = pluginManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            //var dict = await _pluginManager.GetPluginIdAndVersionDictAsync();
            //var list = dict.Keys.ToList();
            //var packageIds = Utilities.ToString(list);

            var plugins = _oldPluginManager.GetPlugins();
            var packagesIds = plugins.Select(x => x.PluginId);
            var enabledPackages = plugins.Select(x => new PackageMetadata(x));

            var pluginIds = _pluginManager.Plugins.Select(x => x.PluginId);
            var enabledPlugins = _pluginManager.Plugins;

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.SdkVersion,
                EnabledPackages = enabledPackages,
                PackageIds = Utilities.ToString(packagesIds),
                EnabledPlugins = enabledPlugins,
                PluginIds = pluginIds
            };
        }

        [HttpDelete, Route(RoutePluginId)]
        public async Task<ActionResult<BoolResult>> Delete(string pluginId)
        {
            if (!await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            _oldPluginManager.Delete(pluginId);
            await _authManager.AddAdminLogAsync("删除插件", $"插件:{pluginId}");

            _cacheManager.Clear();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsReload)]
        public async Task<ActionResult<BoolResult>> Reload()
        {
            if (!await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            //_hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RoutePluginIdEnable)]
        public async Task<ActionResult<BoolResult>> Enable(string pluginId)
        {
            if (!await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = _oldPluginManager.GetPlugin(pluginId);
            if (plugin != null)
            {
                await _pluginRepository.UpdateIsDisabledAsync(pluginId, false);

                await _authManager.AddAdminLogAsync("启用插件", $"插件:{pluginId}");
            }

            _cacheManager.Clear();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
