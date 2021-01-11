using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerPasswordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var userName = request.UserName;
            var adminName = _authManager.AdminName;

            if (string.IsNullOrEmpty(userName)) userName = adminName;
            var adminInfo = await _administratorRepository.GetByUserNameAsync(userName);
            if (adminInfo == null) return NotFound();
            if (userName != adminName &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
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
