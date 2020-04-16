using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Constants.RoleTypeAdministrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersPasswordController : ControllerBase
    {
        private const string Route = "settings/usersPassword";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;

        public UsersPasswordController(IAuthManager authManager, IUserRepository userRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(request.UserId);
            if (user == null) return NotFound();

            return new GetResult
            {
                User = user
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetByUserIdAsync(request.UserId);
            if (user == null) return NotFound();

            var valid = await _userRepository.ChangePasswordAsync(user.Id, request.Password);
            if (!valid.IsValid)
            {
                return this.Error($"更改密码失败：{valid.ErrorMessage}");
            }

            await _authManager.AddAdminLogAsync("重设用户密码", $"用户:{user.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
