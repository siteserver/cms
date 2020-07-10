using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AddLayerUploadController : ControllerBase
    {
        private const string RouteActionsUpload = "plugins/addLayerUpload/actions/upload";
        private const string RouteActionsOverride = "plugins/addLayerUpload/actions/override";
        private const string RouteActionsRestart = "plugins/addLayerUpload/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;

        public AddLayerUploadController(IHostApplicationLifetime hostApplicationLifetime, IAuthManager authManager, IPathManager pathManager, IPluginManager pluginManager)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
        }

        [HttpPost, Route(RouteActionsUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromForm] IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

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
            ZipUtils.ExtractZip(filePath, tempPluginPath);

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
                ZipUtils.ExtractZip(filePath, pluginPath);
            }

            return new UploadResult
            {
                OldPlugin = oldPlugin,
                NewPlugin = plugin,
                FileName = fileName
            };
        }

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

        [HttpPost, Route(RouteActionsRestart)]
        public async Task<ActionResult<BoolResult>> Restart()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            _hostApplicationLifetime.StopApplication();

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
