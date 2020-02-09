using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminView")]
    public partial class PagesAdminViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            auth.CheckAdminLoggin(Request);

            Administrator admin = null;
            if (request.UserId > 0)
            {
                admin = await DataProvider.AdministratorRepository.GetByUserIdAsync(request.UserId);
            }
            else if (!string.IsNullOrEmpty(request.UserName))
            {
                admin = await DataProvider.AdministratorRepository.GetByUserNameAsync(request.UserName);
            }

            if (admin == null)
            {
                return Request.NotFound<GetResult>();
            }

            if (auth.AdminId != admin.Id &&
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdmin))
            {
                return Request.Unauthorized<GetResult>();
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
                    siteNames.Add(await DataProvider.SiteRepository.GetSiteNameAsync(await DataProvider.SiteRepository.GetAsync(siteId)));
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
