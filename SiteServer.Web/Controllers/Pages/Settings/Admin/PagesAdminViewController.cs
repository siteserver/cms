using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminView")]
    public class PagesAdminViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = await AdminManager.GetByUserIdAsync(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var permissions = new PermissionsImpl(adminInfo);
                var level = permissions.GetAdminLevel();
                var isSuperAdmin = permissions.IsConsoleAdministrator;
                var siteNames = new List<string>();
                if (!isSuperAdmin)
                {
                    var siteIdListWithPermissions = permissions.GetSiteIdList();
                    foreach (var siteId in siteIdListWithPermissions)
                    {
                        siteNames.Add(await SiteManager.GetSiteNameAsync(await SiteManager.GetSiteAsync(siteId)));
                    }
                }
                var isOrdinaryAdmin = !permissions.IsSystemAdministrator;
                var roleNames = string.Empty;
                if (isOrdinaryAdmin)
                {
                    roleNames = AdminManager.GetRoles(adminInfo.UserName);
                }
                
                return Ok(new
                {
                    Value = adminInfo,
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
