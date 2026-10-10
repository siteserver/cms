using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<StringResult>> Upload([FromQuery] int userId, [FromForm] IFormFile file)
        {
            var adminId = _authManager.AdminId;
            if (userId > 0 && userId != adminId)
            {
                var target = await _administratorRepository.GetByUserIdAsync(userId);
                if (target == null || !await _authManager.CanManageAdministratorAsync(target))
                {
                    return Unauthorized();
                }
            }
            else if (userId <= 0 &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            if (file == null) return this.Error(Constants.ErrorUpload);
            var fileName = _pathManager.GetUploadFileName(file.FileName);
            var filePath = _pathManager.GetAdministratorUploadPath(userId, fileName);
            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error(Constants.ErrorImageExtensionAllowed);
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
