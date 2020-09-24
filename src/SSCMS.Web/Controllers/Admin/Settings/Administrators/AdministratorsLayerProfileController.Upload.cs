using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int userId, [FromForm] IFormFile file)
        {
            var administrator = await _administratorRepository.GetByUserIdAsync(userId);
            if (administrator == null) return NotFound();

            var adminId = _authManager.AdminId;
            if (adminId != userId &&
                !await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error("请选择有效的文件上传");
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            var filePath = _pathManager.GetAdministratorUploadPath(userId, fileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是图片格式，请选择有效的文件上传!");
            }

            await _pathManager.UploadAsync(file, filePath);

            var avatarUrl = _pathManager.GetAdministratorUploadUrl(userId, fileName);

            return new StringResult
            {
                Value = avatarUrl
            };
        }
    }
}
