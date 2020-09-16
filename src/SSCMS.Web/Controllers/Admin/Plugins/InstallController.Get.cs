using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var pluginIds = ListUtils.GetStringList(request.PluginIds);
            var pluginPathDict = new Dictionary<string, string>();
            foreach (var pluginId in pluginIds)
            {
                var plugin = _pluginManager.GetPlugin(pluginId);
                if (plugin != null)
                {
                    var assemblyPath = plugin.GetAssemblyPath();
                    pluginPathDict[pluginId] = assemblyPath;
                }
            }

            return new GetResult
            {
                Version = _settingsManager.Version,
                PluginPathDict = pluginPathDict
            };
        }
    }
}
