using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerProfileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var userId = request.UserId;

            var adminId = _authManager.AdminId;
            if (adminId != userId &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            Administrator administrator;
            if (userId > 0)
            {
                administrator = await _administratorRepository.GetByUserIdAsync(userId);
                if (administrator == null) return NotFound();
            }
            else
            {
                administrator = new Administrator();
            }

            if (administrator.Id == 0)
            {
                if (!string.IsNullOrEmpty(request.Mobile))
                {
                    var exists = await _administratorRepository.IsMobileExistsAsync(request.Mobile);
                    if (exists)
                    {
                        return this.Error("此手机号码已注册，请更换手机号码");
                    }
                }

                administrator.UserName = request.UserName;
                administrator.CreatorUserName = _authManager.AdminName;
            }
            else
            {
                if (!StringUtils.EqualsIgnoreCase(administrator.Mobile, request.Mobile))
                {
                    if (!string.IsNullOrEmpty(request.Mobile))
                    {
                        var exists = await _administratorRepository.IsMobileExistsAsync(request.Mobile);
                        if (exists)
                        {
                            return this.Error("此手机号码已注册，请更换手机号码");
                        }
                    }

                    administrator.MobileVerified = false;
                }
            }

            administrator.DisplayName = request.DisplayName;
            administrator.AvatarUrl = request.AvatarUrl;
            administrator.Mobile = request.Mobile;
            administrator.Email = request.Email;

            if (administrator.Id == 0)
            {
                var (isValid, errorMessage) = await _administratorRepository.InsertAsync(administrator, request.Password);
                if (!isValid)
                {
                    return this.Error($"管理员添加失败：{errorMessage}");
                }

                if (!string.IsNullOrEmpty(administrator.AvatarUrl))
                {
                    var fileName = PageUtils.GetFileNameFromUrl(administrator.AvatarUrl);
                    var filePath = _pathManager.GetAdministratorUploadPath(0, fileName);
                    var avatarFilePath = _pathManager.GetAdministratorUploadPath(administrator.Id, fileName);
                    FileUtils.CopyFile(filePath, avatarFilePath);
                    administrator.AvatarUrl = _pathManager.GetAdministratorUploadUrl(administrator.Id, fileName);
                    await _administratorRepository.UpdateAsync(administrator);
                }

                await _authManager.AddAdminLogAsync("添加管理员", $"管理员:{administrator.UserName}");
            }
            else
            {
                var (isValid, errorMessage) = await _administratorRepository.UpdateAsync(administrator);
                if (!isValid)
                {
                    return this.Error($"管理员修改失败：{errorMessage}");
                }
                await _authManager.AddAdminLogAsync("修改管理员属性", $"管理员:{administrator.UserName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
