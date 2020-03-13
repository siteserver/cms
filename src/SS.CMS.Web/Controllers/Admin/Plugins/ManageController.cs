using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Packaging;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/manage")]
    public partial class ManageController : ControllerBase
    {
        private const string Route = "";
        private const string RoutePluginId = "{pluginId}";
        private const string RouteActionsReload = "actions/reload";
        private const string RoutePluginIdEnable = "{pluginId}/actions/enable";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly IPluginRepository _pluginRepository;

        public ManageController(IHostApplicationLifetime hostApplicationLifetime, ISettingsManager settingsManager, IAuthManager authManager, IPluginManager pluginManager, IPluginRepository pluginRepository)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
            _pluginRepository = pluginRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            //var dict = await _pluginManager.GetPluginIdAndVersionDictAsync();
            //var list = dict.Keys.ToList();
            //var packageIds = Utilities.ToString(list);

            var plugins = _pluginManager.GetPlugins();
            var packagesIds = plugins.Select(x => x.PluginId);
            var enabledPackages = plugins.Select(x => new PackageMetadata(x));

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.PluginVersion,
                EnabledPackages = enabledPackages,
                PackageIds = Utilities.ToString(packagesIds)
            };
        }

        [HttpDelete, Route(RoutePluginId)]
        public async Task<ActionResult<BoolResult>> Delete(string pluginId)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            _pluginManager.Delete(pluginId);
            await _authManager.AddAdminLogAsync("删除插件", $"插件:{pluginId}");

            CacheUtils.ClearAll();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsReload)]
        public async Task<ActionResult<BoolResult>> Reload()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RoutePluginIdEnable)]
        public async Task<ActionResult<BoolResult>> Enable(string pluginId)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = _pluginManager.GetPlugin(pluginId);
            if (plugin != null)
            {
                await _pluginRepository.UpdateIsDisabledAsync(pluginId, false);

                await _authManager.AddAdminLogAsync("启用插件", $"插件:{pluginId}");
            }

            CacheUtils.ClearAll();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
