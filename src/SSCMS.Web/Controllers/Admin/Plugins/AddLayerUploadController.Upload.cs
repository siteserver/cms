using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class AddLayerUploadController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteActionsUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromForm] IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error(Constants.ErrorUpload);
            }

            var fileName = PathUtils.GetFileName(file.FileName);

            var sExt = PathUtils.GetExtension(fileName);
            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
            {
                return this.Error("插件包为Zip格式，请选择有效的文件上传");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            FileUtils.DeleteFileIfExists(filePath);
            await _pathManager.UploadAsync(file, filePath);

            var tempPluginPath = _pathManager.GetTemporaryFilesPath(PathUtils.GetFileNameWithoutExtension(fileName));
            DirectoryUtils.DeleteDirectoryIfExists(tempPluginPath);
            DirectoryUtils.CreateDirectoryIfNotExists(tempPluginPath);
            _pathManager.ExtractZip(filePath, tempPluginPath);

            var (plugin, errorMessage) = await PluginUtils.ValidateManifestAsync(tempPluginPath);
            if (plugin == null)
            {
                return this.Error(errorMessage);
            }

            DirectoryUtils.DeleteDirectoryIfExists(tempPluginPath);

            var oldPlugin = _pluginManager.GetPlugin(plugin.PluginId);

            if (oldPlugin == null)
            {
                var pluginPath = _pathManager.GetPluginPath(plugin.PluginId);
                DirectoryUtils.DeleteDirectoryIfExists(pluginPath);
                DirectoryUtils.CreateDirectoryIfNotExists(pluginPath);
                _pathManager.ExtractZip(filePath, pluginPath);
            }

            return new UploadResult
            {
                OldPlugin = oldPlugin,
                NewPlugin = plugin,
                FileName = fileName
            };
        }
    }
}
