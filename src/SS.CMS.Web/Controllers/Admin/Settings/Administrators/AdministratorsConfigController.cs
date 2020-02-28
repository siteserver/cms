using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsConfig")]
    public partial class AdministratorsConfigController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public AdministratorsConfigController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Config = config
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.AdminUserNameMinLength = request.AdminUserNameMinLength;
            config.AdminPasswordMinLength = request.AdminPasswordMinLength;
            config.AdminPasswordRestriction = request.AdminPasswordRestriction;

            config.IsAdminLockLogin = request.IsAdminLockLogin;
            config.AdminLockLoginCount = request.AdminLockLoginCount;
            config.AdminLockLoginType = request.AdminLockLoginType;
            config.AdminLockLoginHours = request.AdminLockLoginHours;

            config.IsAdminEnforcePasswordChange = request.IsAdminEnforcePasswordChange;
            config.AdminEnforcePasswordChangeDays = request.AdminEnforcePasswordChangeDays;

            config.IsAdminEnforceLogout = request.IsAdminEnforceLogout;
            config.AdminEnforceLogoutMinutes = request.AdminEnforceLogoutMinutes;

            await _configRepository.UpdateAsync(config);

            await auth.AddAdminLogAsync("修改管理员设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
