using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteActionsDownload)]
        public async Task<ActionResult<BoolResult>> Download([FromBody] DownloadRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.PluginsAdd))
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

            _pluginManager.Install(request.PluginId, request.Version);

            await _authManager.AddAdminLogAsync("安装插件", $"插件:{request.PluginId}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
