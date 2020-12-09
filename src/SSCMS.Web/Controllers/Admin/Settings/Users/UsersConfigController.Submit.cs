using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersConfigController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.IsUserRegistrationAllowed = request.IsUserRegistrationAllowed;
            config.IsUserRegistrationChecked = request.IsUserRegistrationChecked;
            config.IsUserUnRegistrationAllowed = request.IsUserUnRegistrationAllowed;
            config.IsUserForceVerifyMobile = request.IsUserForceVerifyMobile;
            config.UserPasswordMinLength = request.UserPasswordMinLength;
            config.UserPasswordRestriction = request.UserPasswordRestriction;
            config.UserRegistrationMinMinutes = request.UserRegistrationMinMinutes;
            config.IsUserLockLogin = request.IsUserLockLogin;
            config.UserLockLoginCount = request.UserLockLoginCount;
            config.UserLockLoginType = request.UserLockLoginType;
            config.UserLockLoginHours = request.UserLockLoginHours;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改用户设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
