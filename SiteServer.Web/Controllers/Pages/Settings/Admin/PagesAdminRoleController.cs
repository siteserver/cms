using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.Utils;

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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var roleInfoList = await request.AdminPermissionsImpl.IsSuperAdminAsync()
                    ? await DataProvider.RoleDao.GetRoleListAsync()
                    : await DataProvider.RoleDao.GetRoleListByCreatorUserNameAsync(request.AdminName);

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

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var roleInfo = await DataProvider.RoleDao.GetRoleAsync(id);

                await DataProvider.PermissionsInRolesDao.DeleteAsync(roleInfo.RoleName);
                await DataProvider.SitePermissionsDao.DeleteAsync(roleInfo.RoleName);
                await DataProvider.RoleDao.DeleteRoleAsync(roleInfo.Id);

                await request.AddAdminLogAsync("删除管理员角色", $"角色名称:{roleInfo.RoleName}");

                var roleInfoList = await request.AdminPermissionsImpl.IsSuperAdminAsync()
                    ? await DataProvider.RoleDao.GetRoleListAsync()
                    : await DataProvider.RoleDao.GetRoleListByCreatorUserNameAsync(request.AdminName);

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
