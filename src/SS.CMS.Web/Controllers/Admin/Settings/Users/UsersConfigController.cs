using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersConfig")]
    public partial class UsersConfigController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public UsersConfigController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersConfig))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            return new GetResult
            {
                Config = config
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsersConfig))
            {
                return Unauthorized();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            config.IsUserRegistrationAllowed = request.IsUserRegistrationAllowed;
            config.IsUserRegistrationChecked = request.IsUserRegistrationChecked;
            config.IsUserUnRegistrationAllowed = request.IsUserUnRegistrationAllowed;
            config.UserPasswordMinLength = request.UserPasswordMinLength;
            config.UserPasswordRestriction = request.UserPasswordRestriction;
            config.UserRegistrationMinMinutes = request.UserRegistrationMinMinutes;
            config.IsUserLockLogin = request.IsUserLockLogin;
            config.UserLockLoginCount = request.UserLockLoginCount;
            config.UserLockLoginType = request.UserLockLoginType;
            config.UserLockLoginHours = request.UserLockLoginHours;

            await DataProvider.ConfigRepository.UpdateAsync(config);

            await auth.AddAdminLogAsync("修改用户设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
