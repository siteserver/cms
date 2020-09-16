using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class InstallController
    {
        [HttpPost, Route(RouteActionsUpdate)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UploadRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.PluginsAdd))
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
