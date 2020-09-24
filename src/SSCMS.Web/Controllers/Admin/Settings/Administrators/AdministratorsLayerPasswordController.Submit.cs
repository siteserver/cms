using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerPasswordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var userId = request.UserId;
            var adminId = _authManager.AdminId;

            if (userId == 0) userId = adminId;
            var adminInfo = await _administratorRepository.GetByUserIdAsync(userId);
            if (adminInfo == null) return NotFound();
            if (adminId != userId &&
                !await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var password = request.Password;
            var valid = await _administratorRepository.ChangePasswordAsync(adminInfo, password);
            if (!valid.IsValid)
            {
                return this.Error($"更改密码失败：{valid.ErrorMessage}");
            }

            await _authManager.AddAdminLogAsync("重设管理员密码", $"管理员:{adminInfo.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
