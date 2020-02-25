using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsView")]
    public partial class AdministratorsViewController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public AdministratorsViewController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin)
            {
                return Unauthorized();
            }

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
                return NotFound();
            }

            if (auth.AdminId != admin.Id &&
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
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
                SiteNames = string.Join("<br />", siteNames),
                IsOrdinaryAdmin = isOrdinaryAdmin,
                RoleNames = roleNames
            };
        }
    }
}
