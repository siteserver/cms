using System;
using System.Linq;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
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
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var roleInfoList = request.AdminPermissionsImpl.IsConsoleAdministrator
                    ? DataProvider.RoleDao.GetRoleInfoList()
                    : DataProvider.RoleDao.GetRoleInfoListByCreatorUserName(request.AdminName);

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
        public IHttpActionResult Delete()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminRole))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                var roleInfo = DataProvider.RoleDao.GetRoleInfo(id);

                DataProvider.PermissionsInRolesDao.Delete(roleInfo.RoleName);
                DataProvider.SitePermissionsDao.Delete(roleInfo.RoleName);
                DataProvider.RoleDao.DeleteRole(roleInfo.Id);

                request.AddAdminLog("删除管理员角色", $"角色名称:{roleInfo.RoleName}");

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
