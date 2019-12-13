using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminRole")]
    public class PagesAdminRoleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetList()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleInfoList = await request.AdminPermissionsImpl.IsSuperAdminAsync()
                    ? await DataProvider.RoleRepository.GetRoleListAsync()
                    : await DataProvider.RoleRepository.GetRoleListByCreatorUserNameAsync(request.AdminName);

                var roles = roleInfoList.Where(x => !EPredefinedRoleUtils.IsPredefinedRole(x.RoleName)).ToList();

                return Ok(new
                {
                    Value = roles
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var roleInfo = await DataProvider.RoleRepository.GetRoleAsync(id);

                await DataProvider.PermissionsInRolesRepository.DeleteAsync(roleInfo.RoleName);
                await DataProvider.SitePermissionsRepository.DeleteAsync(roleInfo.RoleName);
                await DataProvider.RoleRepository.DeleteRoleAsync(roleInfo.Id);

                await request.AddAdminLogAsync("删除管理员角色", $"角色名称:{roleInfo.RoleName}");

                var roleInfoList = await request.AdminPermissionsImpl.IsSuperAdminAsync()
                    ? await DataProvider.RoleRepository.GetRoleListAsync()
                    : await DataProvider.RoleRepository.GetRoleListByCreatorUserNameAsync(request.AdminName);

                return Ok(new
                {
                    Value = roleInfoList
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
