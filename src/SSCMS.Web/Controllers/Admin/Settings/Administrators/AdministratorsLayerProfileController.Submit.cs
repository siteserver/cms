using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

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
                !await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
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
                administrator.UserName = request.UserName;
                administrator.CreatorUserName = _authManager.AdminName;
            }
            else
            {
                if (administrator.Mobile != request.Mobile && !string.IsNullOrEmpty(request.Mobile) && await _administratorRepository.IsMobileExistsAsync(request.Mobile))
                {
                    return this.Error("资料修改失败，手机号码已存在");
                }

                if (administrator.Email != request.Email && !string.IsNullOrEmpty(request.Email) && await _administratorRepository.IsEmailExistsAsync(request.Email))
                {
                    return this.Error("资料修改失败，邮箱地址已存在");
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
