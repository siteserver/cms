using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminRole")]
    public class PagesAdminRoleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetList()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var roleInfoList = request.AdminPermissionsImpl.IsConsoleAdministrator
                    ? DataProvider.RoleDao.GetRoleInfoList()
                    : DataProvider.RoleDao.GetRoleInfoListByCreatorUserName(request.AdminName);

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
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var roleInfo = DataProvider.RoleDao.GetRoleInfo(id);

                DataProvider.PermissionsInRolesDao.Delete(roleInfo.RoleName);
                DataProvider.SitePermissionsDao.Delete(roleInfo.RoleName);
                DataProvider.RoleDao.DeleteRole(roleInfo.Id);

                await request.AddAdminLogAsync("删除管理员角色", $"角色名称:{roleInfo.RoleName}");

                var roleInfoList = request.AdminPermissionsImpl.IsConsoleAdministrator
                    ? DataProvider.RoleDao.GetRoleInfoList()
                    : DataProvider.RoleDao.GetRoleInfoListByCreatorUserName(request.AdminName);

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
