﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/view")]
    public partial class ViewController : ControllerBase
    {
        private const string Route = "{pluginId}";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;

        public ViewController(ISettingsManager settingsManager, IAuthManager authManager, IPluginManager pluginManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pluginManager = pluginManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get(string pluginId)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsManagement))
            {
                return Unauthorized();
            }

            var plugin = await _pluginManager.GetPluginAsync(pluginId);

            return new GetResult
            {
                IsNightly = _settingsManager.IsNightlyUpdate,
                PluginVersion = _settingsManager.PluginVersion,
                Installed = plugin != null,
                InstalledVersion = plugin != null ? plugin.Version : string.Empty,
                Package = plugin
            };
        }
    }
}
