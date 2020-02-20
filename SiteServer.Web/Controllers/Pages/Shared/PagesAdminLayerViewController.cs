using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/adminLayerView")]
    public partial class PagesAdminLayerViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GetResult>();

            var admin = await DataProvider.AdministratorRepository.GetByUserIdAsync(request.AdminId);

            if (admin == null) return Request.NotFound<GetResult>();

            var permissions = new PermissionsImpl(admin);
            var level = await permissions.GetAdminLevelAsync();
            var isSuperAdmin = await permissions.IsSuperAdminAsync();
            var siteNames = new List<string>();
            if (!isSuperAdmin)
            {
                var siteIdListWithPermissions = await permissions.GetSiteIdListAsync();
                foreach (var siteId in siteIdListWithPermissions)
                {
                    var site = await DataProvider.SiteRepository.GetAsync(siteId);
                    siteNames.Add(site.SiteName);
                }
            }
            var isOrdinaryAdmin = !await permissions.IsSuperAdminAsync();
            var roleNames = string.Empty;
            if (isOrdinaryAdmin)
            {
                roleNames = await DataProvider.AdministratorRepository.GetRolesAsync(admin.UserName);
            }

            return new GetResult
            {
                Administrator = admin,
                Level = level,
                IsSuperAdmin = isSuperAdmin,
                SiteNames = Utilities.ToString(siteNames, "<br />"),
                IsOrdinaryAdmin = isOrdinaryAdmin,
                RoleNames = roleNames
            };
        }
    }
}
