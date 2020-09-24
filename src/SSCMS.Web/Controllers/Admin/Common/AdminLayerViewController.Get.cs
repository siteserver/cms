using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Services;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class AdminLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            Administrator admin = null;
            if (request.AdminId > 0)
            {
                admin = await _administratorRepository.GetByUserIdAsync(request.AdminId);
            }
            else if (!string.IsNullOrEmpty(request.UserName))
            {
                admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
            }

            if (admin == null) return NotFound();

            var permissions = new AuthManager(_context, _settingsManager, _databaseManager);
            permissions.Init(admin);
            var level = await permissions.GetAdminLevelAsync();
            var isSuperAdmin = await permissions.IsSuperAdminAsync();
            var siteNames = new List<string>();
            if (!isSuperAdmin)
            {
                var siteIdListWithPermissions = await permissions.GetSiteIdsAsync();
                foreach (var siteId in siteIdListWithPermissions)
                {
                    var site = await _siteRepository.GetAsync(siteId);
                    siteNames.Add(site.SiteName);
                }
            }
            var isOrdinaryAdmin = !await permissions.IsSuperAdminAsync();
            var roleNames = string.Empty;
            if (isOrdinaryAdmin)
            {
                roleNames = await _administratorRepository.GetRolesAsync(admin.UserName);
            }

            return new GetResult
            {
                Administrator = admin,
                Level = level,
                IsSuperAdmin = isSuperAdmin,
                SiteNames = ListUtils.ToString(siteNames, "<br />"),
                IsOrdinaryAdmin = isOrdinaryAdmin,
                RoleNames = roleNames
            };
        }
    }
}
