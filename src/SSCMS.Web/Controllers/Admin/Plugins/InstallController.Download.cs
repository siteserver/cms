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

            if (!string.IsNullOrEmpty(request.Path))
            {
                if (!FileUtils.DeleteFileIfExists(request.Path))
                {
                    return new BoolResult
                    {
                        Value = false
                    };
                }
            }

            var userName = request.PluginId.Split('.')[0];
            var name = request.PluginId.Split('.')[1];

            _pluginManager.Install(userName, name, request.Version);

            await _authManager.AddAdminLogAsync("安装插件", $"插件:{userName}.{name}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
