using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersPassword")]
    public partial class UsersPasswordController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public UsersPasswordController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await DataProvider.UserRepository.GetByUserIdAsync(request.UserId);
            if (user == null) return NotFound();

            return new GetResult
            {
                User = user
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await DataProvider.UserRepository.GetByUserIdAsync(request.UserId);
            if (user == null) return NotFound();

            var valid = await DataProvider.UserRepository.ChangePasswordAsync(user.Id, request.Password);
            if (!valid.IsValid)
            {
                return this.Error($"更改密码失败：{valid.ErrorMessage}");
            }

            await auth.AddAdminLogAsync("重设用户密码", $"用户:{user.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
