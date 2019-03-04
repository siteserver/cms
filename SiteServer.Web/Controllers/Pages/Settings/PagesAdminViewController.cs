using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/adminView")]
    public class PagesAdminViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new RequestImpl();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var departmentName = DepartmentManager.GetDepartmentName(adminInfo.DepartmentId);
                var areaName = AreaManager.GetAreaName(adminInfo.AreaId);

                var permissions = new PermissionsImpl(adminInfo);
                var level = permissions.GetAdminLevel();
                var isSuperAdmin = permissions.IsConsoleAdministrator;
                var siteNames = new List<string>();
                if (!isSuperAdmin)
                {
                    var siteIdListWithPermissions = permissions.GetSiteIdList();
                    foreach (var siteId in siteIdListWithPermissions)
                    {
                        siteNames.Add(SiteManager.GetSiteName(SiteManager.GetSiteInfo(siteId)));
                    }
                }
                var isOrdinaryAdmin = !permissions.IsSystemAdministrator;
                var roleNames = string.Empty;
                if (isOrdinaryAdmin)
                {
                    roleNames = AdminManager.GetRolesHtml(adminInfo.UserName);
                }
                
                return Ok(new
                {
                    Value = adminInfo,
                    DepartmentName = departmentName,
                    AreaName = areaName,
                    Level = level,
                    IsSuperAdmin = isSuperAdmin,
                    SiteNames = TranslateUtils.ObjectCollectionToString(siteNames, "<br />"),
                    IsOrdinaryAdmin = isOrdinaryAdmin,
                    RoleNames = roleNames
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
