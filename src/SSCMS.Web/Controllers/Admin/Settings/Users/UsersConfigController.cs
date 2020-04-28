using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersConfigController : ControllerBase
    {
        private const string Route = "settings/usersConfig";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;

        public UsersConfigController(IAuthManager authManager, IConfigRepository configRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsersConfig))
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
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsUsersConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

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

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改用户设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
