using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ProfileController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromForm] IFormFile file)
        {
            if (file == null) return this.Error("请选择有效的文件上传");
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            var filePath = _pathManager.GetUserUploadPath(_authManager.UserId, fileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }

            await _pathManager.UploadAsync(file, filePath);

            var avatarUrl = _pathManager.GetUserUploadUrl(_authManager.UserId, fileName);

            return new StringResult
            {
                Value = avatarUrl
            };
        }
    }
}
