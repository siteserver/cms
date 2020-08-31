using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost, Route(RouteUserAvatar)]
        public async Task<ActionResult<User>> UploadAvatar([FromQuery] int id, [FromForm] IFormFile file)
        {
            var isAuth = await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers) ||
                         _authManager.IsUser &&
                         _authManager.UserId == id ||
                         _authManager.IsAdmin &&
                         await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers);
            if (!isAuth) return Unauthorized();

            var user = await _userRepository.GetByUserIdAsync(id);
            if (user == null) return NotFound();

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = PathUtils.GetFileName(file.FileName);

            fileName = _pathManager.GetUserUploadFileName(fileName);
            var filePath = _pathManager.GetUserUploadPath(user.Id, fileName);

            if (!FileUtils.IsImage(PathUtils.GetExtension(fileName)))
            {
                return this.Error("文件只能是 Image 格式，请选择有效的文件上传");
            }

            await _pathManager.UploadAsync(file, filePath);

            user.AvatarUrl = _pathManager.GetUserUploadUrl(user.Id, fileName);

            await _userRepository.UpdateAsync(user);

            return user;
        }
    }
}
