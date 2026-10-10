using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteActionsDownload)]
        public async Task<ActionResult<BoolResult>> Download([FromBody] DownloadRequest request)
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

            var path = PathUtils.RemoveParentPath(request.Path);
            if (!string.IsNullOrEmpty(path))
            {
                // 前端传入的是旧版插件程序集路径：仅允许删除该插件自身目录内的文件，防止传入绝对路径删除任意文件
                if (!DirectoryUtils.IsInDirectory(_pathManager.GetPluginPath(request.PluginId), path))
                {
                    return this.Error("路径不合法");
                }
                if (!FileUtils.DeleteFileIfExists(path))
                {
                    return new BoolResult
                    {
                        Value = false
                    };
                }
            }

            var userName = request.PluginId.Split('.')[0];
            var name = request.PluginId.Split('.')[1];

            var downloadUrl = CloudUtils.Dl.GetExtensionsDownloadUrl(userName, name, request.Version);
            if (await _cloudManager.IsAuthenticationAsync())
            {
                downloadUrl = await _cloudManager.GetExtensionDownloadUrlAsync(userName, name, request.Version);
            }

            await _pluginManager.InstallAsync(userName, name, request.Version, downloadUrl);

            await _authManager.AddAdminLogAsync("安装插件", $"插件:{userName}.{name}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
