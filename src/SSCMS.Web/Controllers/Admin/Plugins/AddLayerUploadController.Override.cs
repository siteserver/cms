using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
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
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            var pluginPath = _pathManager.GetPluginPath(request.PluginId);
            var configPath = PathUtils.Combine(pluginPath, Constants.PluginConfigFileName);
            var configValue = string.Empty;
            if (FileUtils.IsFileExists(configPath))
            {
                configValue = await FileUtils.ReadTextAsync(configPath);
            }

            DirectoryUtils.DeleteDirectoryIfExists(pluginPath);
            DirectoryUtils.CreateDirectoryIfNotExists(pluginPath);
            _pathManager.ExtractZip(filePath, pluginPath);
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
