using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteActionsUpdate)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UploadRequest request)
        {
            // 插件即任意代码执行：上传/覆盖/安装/重启仅限超级管理员（#3865）
            if (!await _authManager.IsSuperAdminAsync() || !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var idWithVersion = $"{request.PluginId}.{request.Version}";
            //if (!_pluginManager.UpdatePackage(idWithVersion, TranslateUtils.ToEnum(request.PackageType, PackageType.Library), out var errorMessage))
            //{
            //    return this.Error(errorMessage);
            //}

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
