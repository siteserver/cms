using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsRole")]
    public partial class AdministratorsRoleController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public AdministratorsRoleController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListRequest>> GetList()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfoList = await auth.AdminPermissions.IsSuperAdminAsync()
                ? await DataProvider.RoleRepository.GetRoleListAsync()
                : await DataProvider.RoleRepository.GetRoleListByCreatorUserNameAsync(auth.AdminName);

            var roles = roleInfoList.Where(x => !DataProvider.RoleRepository.IsPredefinedRole(x.RoleName)).ToList();

            return new ListRequest
            {
                Roles = roles
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<ListRequest>> Delete([FromBody] IdRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            var roleInfo = await DataProvider.RoleRepository.GetRoleAsync(request.Id);

            await DataProvider.PermissionsInRolesRepository.DeleteAsync(roleInfo.RoleName);
            await DataProvider.SitePermissionsRepository.DeleteAsync(roleInfo.RoleName);
            await DataProvider.RoleRepository.DeleteRoleAsync(roleInfo.Id);

            await auth.AddAdminLogAsync("删除管理员角色", $"角色名称:{roleInfo.RoleName}");

            var roles = await auth.AdminPermissions.IsSuperAdminAsync()
                ? await DataProvider.RoleRepository.GetRoleListAsync()
                : await DataProvider.RoleRepository.GetRoleListByCreatorUserNameAsync(auth.AdminName);

            return new ListRequest
            {
                Roles = roles
            };
        }
    }
}
