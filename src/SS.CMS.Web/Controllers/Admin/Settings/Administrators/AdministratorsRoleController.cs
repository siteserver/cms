using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsRole")]
    public partial class AdministratorsRoleController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IRoleRepository _roleRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;

        public AdministratorsRoleController(IAuthManager authManager, IRoleRepository roleRepository, ISitePermissionsRepository sitePermissionsRepository, IPermissionsInRolesRepository permissionsInRolesRepository)
        {
            _authManager = authManager;
            _roleRepository = roleRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListRequest>> GetList()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfoList = await _authManager.IsSuperAdminAsync()
                ? await _roleRepository.GetRoleListAsync()
                : await _roleRepository.GetRoleListByCreatorUserNameAsync(await _authManager.GetAdminNameAsync());

            var roles = roleInfoList.Where(x => !_roleRepository.IsPredefinedRole(x.RoleName)).ToList();

            return new ListRequest
            {
                Roles = roles
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<ListRequest>> Delete([FromBody] IdRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfo = await _roleRepository.GetRoleAsync(request.Id);

            await _permissionsInRolesRepository.DeleteAsync(roleInfo.RoleName);
            await _sitePermissionsRepository.DeleteAsync(roleInfo.RoleName);
            await _roleRepository.DeleteRoleAsync(roleInfo.Id);

            await _authManager.AddAdminLogAsync("删除管理员角色", $"角色名称:{roleInfo.RoleName}");

            var roles = await _authManager.IsSuperAdminAsync()
                ? await _roleRepository.GetRoleListAsync()
                : await _roleRepository.GetRoleListByCreatorUserNameAsync(await _authManager.GetAdminNameAsync());

            return new ListRequest
            {
                Roles = roles
            };
        }
    }
}
