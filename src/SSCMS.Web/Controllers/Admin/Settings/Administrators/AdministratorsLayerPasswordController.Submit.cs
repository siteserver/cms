using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Configuration;

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
            if (adminInfo == null) return this.Error(Constants.ErrorNotFound);
            if (userName != adminName &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var password = StringUtils.Base64Decode(request.Password);
            var (isValid, errorMessage) = await _administratorRepository.ChangePasswordAsync(adminInfo, password);
            if (!isValid)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            await _authManager.AddAdminLogAsync("重设管理员密码", $"管理员:{adminInfo.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
