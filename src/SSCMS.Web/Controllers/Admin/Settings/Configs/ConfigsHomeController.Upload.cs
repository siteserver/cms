using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm] IFormFile file)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }
            var filePath = _pathManager.GetHomeUploadPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            var url = _pathManager.GetHomeUploadUrl(fileName);

            return new StringResult
            {
                Value = url
            };
        }
    }
}
