using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class AddLayerUploadController
    {
        [HttpPost, Route(RouteActionsOverride)]
        public async Task<ActionResult<BoolResult>> Override([FromBody] OverrideRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var fileName = PathUtils.RemoveParentPath(request.FileName);
            var tempPluginPath = _pathManager.GetTemporaryFilesPath(PathUtils.GetFileNameWithoutExtension(fileName));
            var (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(tempPluginPath);
            if (plugin == null)
            {
                return this.Error(errorMessage);
            }

            if (!StringUtils.EqualsIgnoreCase(plugin.PluginId, request.PluginId))
            {
                return this.Error("插件包与插件Id不匹配");
            }

            var pluginPath = _pathManager.GetPluginPath(request.PluginId);
            var configPath = PathUtils.Combine(pluginPath, Constants.PluginConfigFileName);
            var configValue = string.Empty;
            if (FileUtils.IsFileExists(configPath))
            {
                configValue = await FileUtils.ReadTextAsync(configPath);
            }

            DirectoryUtils.DeleteDirectoryIfExists(pluginPath);
            DirectoryUtils.MoveDirectory(tempPluginPath, pluginPath, true);
            DirectoryUtils.DeleteDirectoryIfExists(tempPluginPath);
            if (!string.IsNullOrEmpty(configValue))
            {
                await FileUtils.WriteTextAsync(configPath, configValue);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
