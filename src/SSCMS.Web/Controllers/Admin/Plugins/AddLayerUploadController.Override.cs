using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class AddLayerUploadController
    {
        [HttpPost, Route(RouteActionsOverride)]
        public async Task<ActionResult<BoolResult>> Override([FromBody] OverrideRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            _pluginManager.UnInstall(request.PluginId);

            var filePath = _pathManager.GetTemporaryFilesPath(request.FileName);
            var pluginPath = _pathManager.GetPluginPath(request.PluginId);
            DirectoryUtils.CreateDirectoryIfNotExists(pluginPath);
            ZipUtils.ExtractZip(filePath, pluginPath);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
