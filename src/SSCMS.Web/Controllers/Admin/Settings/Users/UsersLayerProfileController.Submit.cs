using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerProfileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] User request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            if (request.Id == 0)
            {
                if (!string.IsNullOrEmpty(request.Mobile))
                {
                    var exists = await _userRepository.IsMobileExistsAsync(request.Mobile);
                    if (exists)
                    {
                        return this.Error("此手机号码已注册，请更换手机号码");
                    }
                }

                var (user, errorMessage) = await _userRepository.InsertAsync(request, request.Password, string.Empty);
                if (user == null)
                {
                    return this.Error($"用户添加失败：{errorMessage}");
                }

                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    var fileName = PageUtils.GetFileNameFromUrl(user.AvatarUrl);
                    var filePath = _pathManager.GetUserUploadPath(0, fileName);
                    var avatarFilePath = _pathManager.GetUserUploadPath(user.Id, fileName);
                    FileUtils.CopyFile(filePath, avatarFilePath);
                    user.AvatarUrl = _pathManager.GetUserUploadUrl(user.Id, fileName);
                    await _userRepository.UpdateAsync(user);
                }

                await _authManager.AddAdminLogAsync("添加用户", $"用户:{request.UserName}");
            }
            else
            {
                var user = await _userRepository.GetByUserIdAsync(request.Id);
                if (!StringUtils.EqualsIgnoreCase(user.Mobile, request.Mobile))
                {
                    if (!string.IsNullOrEmpty(request.Mobile))
                    {
                        var exists = await _userRepository.IsMobileExistsAsync(request.Mobile);
                        if (exists)
                        {
                            return this.Error("此手机号码已注册，请更换手机号码");
                        }
                    }

                    request.MobileVerified = false;
                }

                var (success, errorMessage) = await _userRepository.UpdateAsync(request);
                if (!success)
                {
                    return this.Error($"用户修改失败：{errorMessage}");
                }

                await _authManager.AddAdminLogAsync("修改用户", $"用户:{request.UserName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
