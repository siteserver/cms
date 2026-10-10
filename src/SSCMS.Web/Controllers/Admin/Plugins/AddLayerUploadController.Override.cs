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
            // 插件即任意代码执行：上传/覆盖/安装/重启仅限超级管理员（#3865）
            if (!await _authManager.IsSuperAdminAsync() || !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            if (!PluginUtils.IsValidPluginId(_pathManager.PluginsPath, request.PluginId))
            {
                return this.Error("插件 ID 不合法");
            }
            var fileName = PathUtils.GetFileName(PathUtils.RemoveParentPath(request.FileName));
            if (string.IsNullOrEmpty(fileName) || !StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(fileName), ".zip"))
            {
                return this.Error("插件包为Zip格式，请选择有效的文件上传");
            }
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            if (!FileUtils.IsFileExists(filePath))
            {
                return this.Error("插件包不存在，请重新上传");
            }
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

            await _authManager.AddAdminLogAsync("覆盖上传插件", $"插件:{request.PluginId}，文件:{fileName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
