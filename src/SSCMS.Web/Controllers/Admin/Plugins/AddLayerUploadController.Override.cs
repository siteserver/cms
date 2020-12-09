using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            _pluginManager.UnInstall(request.PluginId);

            var filePath = _pathManager.GetTemporaryFilesPath(request.FileName);
            var pluginPath = _pathManager.GetPluginPath(request.PluginId);
            DirectoryUtils.CreateDirectoryIfNotExists(pluginPath);
            _pathManager.ExtractZip(filePath, pluginPath);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
