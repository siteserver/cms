using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Plugins;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/manage")]
    public partial class ManageController : ControllerBase
    {
        private const string Route = "";
        private const string RoutePluginId = "{pluginId}";
        private const string RouteActionsReload = "actions/reload";
        private const string RoutePluginIdEnable = "{pluginId}/actions/enable";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginRepository _pluginRepository;

        public ManageController(ISettingsManager settingsManager, IAuthManager authManager, IPluginRepository pluginRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginRepository = pluginRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var dict = await PluginManager.GetPluginIdAndVersionDictAsync();
            var list = dict.Keys.ToList();
            var packageIds = Utilities.ToString(list);

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.PluginVersion,
                AllPackages = await PluginManager.GetAllPluginInfoListAsync(),
                PackageIds = packageIds
            };
        }

        [HttpDelete, Route(RoutePluginId)]
        public async Task<ActionResult<BoolResult>> Delete(string pluginId)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            PluginManager.Delete(pluginId);
            await auth.AddAdminLogAsync("删除插件", $"插件:{pluginId}");

            CacheUtils.ClearAll();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteActionsReload)]
        public async Task<ActionResult<BoolResult>> Reload()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            CacheUtils.ClearAll();

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RoutePluginIdEnable)]
        public async Task<ActionResult<BoolResult>> Enable(string pluginId)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var pluginInfo = await PluginManager.GetPluginInfoAsync(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.IsDisabled = !pluginInfo.IsDisabled;
                await _pluginRepository.UpdateIsDisabledAsync(pluginId, pluginInfo.IsDisabled);
                PluginManager.ClearCache();

                await auth.AddAdminLogAsync(!pluginInfo.IsDisabled ? "禁用插件" : "启用插件", $"插件:{pluginId}");
            }

            CacheUtils.ClearAll();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
