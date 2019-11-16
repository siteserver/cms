using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
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
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin) return Unauthorized();

                var userId = request.GetQueryInt("userId");
                var userName = request.GetQueryString("userName");

                Administrator admin = null;
                if (userId > 0)
                {
                    admin = await AdminManager.GetByUserIdAsync(userId);
                }
                else if (!string.IsNullOrEmpty(userName))
                {
                    admin = await AdminManager.GetByUserNameAsync(userName);
                }

                if (admin == null) return NotFound();
                if (request.AdminId != admin.Id &&
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var permissions = new PermissionsImpl(admin);
                var level = await permissions.GetAdminLevelAsync();
                var isSuperAdmin = await permissions.IsSuperAdminAsync();
                var siteNames = new List<string>();
                if (!isSuperAdmin)
                {
                    var siteIdListWithPermissions = await permissions.GetSiteIdListAsync();
                    foreach (var siteId in siteIdListWithPermissions)
                    {
                        siteNames.Add(await SiteManager.GetSiteNameAsync(await SiteManager.GetSiteAsync(siteId)));
                    }
                }
                var isOrdinaryAdmin = !await permissions.IsSuperAdminAsync();
                var roleNames = string.Empty;
                if (isOrdinaryAdmin)
                {
                    roleNames = await AdminManager.GetRolesAsync(admin.UserName);
                }
                
                return Ok(new
                {
                    Value = admin,
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
