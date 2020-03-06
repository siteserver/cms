using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/addLayerUpload")]
    public partial class AddLayerUploadController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;

        public AddLayerUploadController(IAuthManager authManager, IPathManager pathManager)
        {
            _authManager = authManager;
            _pathManager = pathManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<BoolResult>> GetConfig()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromForm]IFormFile file)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            string filePath = null;

            var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
            if (extendName == ".nupkg")
            {
                filePath = _pathManager.GetTemporaryFilesPath(fileName);
                await _pathManager.UploadAsync(file, filePath);
            }

            FileInfo fileInfo = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileInfo = new FileInfo(filePath);
            }
            if (fileInfo != null)
            {
                return new UploadResult
                {
                    FileName = fileName,
                    Length = fileInfo.Length,
                    Ret = 1
                };
            }

            return new UploadResult
            {
                Ret = 0
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            foreach (var fileName in request.FileNames)
            {
                //var localFilePath = _pathManager.GetTemporaryFilesPath(fileName);

                //var importObject = new ImportObject(siteId, request.AdminName);
                //importObject.ImportContentsByZipFile(channel, localFilePath, isOverride, isChecked, checkedLevel, request.AdminId, 0, SourceManager.Default);
            }

            await auth.AddAdminLogAsync("安装离线插件", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
