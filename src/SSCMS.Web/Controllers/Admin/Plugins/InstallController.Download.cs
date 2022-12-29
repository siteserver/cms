using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var path = PathUtils.RemoveParentPath(request.Path);
            if (!string.IsNullOrEmpty(path))
            {
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
