using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Plugins
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AddLayerUploadController : ControllerBase
    {
        private const string Route = "plugins/addLayerUpload";
        private const string RouteUpload = "plugins/addLayerUpload/actions/upload";

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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
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
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            foreach (var fileName in request.FileNames)
            {
                //var localFilePath = _pathManager.GetTemporaryFilesPath(fileName);

                //var importObject = new ImportObject(siteId, request.AdminName);
                //importObject.ImportContentsByZipFile(channel, localFilePath, isOverride, isChecked, checkedLevel, request.AdminId, 0, SourceManager.Default);
            }

            await _authManager.AddAdminLogAsync("安装离线插件", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
