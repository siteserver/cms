using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsLayerPasswordController : ControllerBase
    {
        private const string Route = "settings/administratorsLayerPassword";

        private readonly IAuthManager _authManager;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsLayerPasswordController(IAuthManager authManager, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var userId = request.UserId;
            var adminId = _authManager.AdminId;

            if (userId == 0) userId = adminId;
            var administrator = await _administratorRepository.GetByUserIdAsync(userId);
            if (administrator == null) return NotFound();
            if (adminId != userId &&
                !await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            return new GetResult
            {
                Administrator = administrator
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var userId = request.UserId;
            var adminId = _authManager.AdminId;

            if (userId == 0) userId = adminId;
            var adminInfo = await _administratorRepository.GetByUserIdAsync(userId);
            if (adminInfo == null) return NotFound();
            if (adminId != userId &&
                !await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAdministrators))
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
