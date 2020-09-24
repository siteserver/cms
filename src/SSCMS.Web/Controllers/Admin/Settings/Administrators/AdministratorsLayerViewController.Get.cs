using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Services;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            Administrator admin = null;
            if (request.UserId > 0)
            {
                admin = await _administratorRepository.GetByUserIdAsync(request.UserId);
            }
            else if (!string.IsNullOrEmpty(request.UserName))
            {
                admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
            }

            if (admin == null)
            {
                return NotFound();
            }

            var adminId = _authManager.AdminId;
            if (adminId != admin.Id &&
                !await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

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
                SiteNames = string.Join("<br />", siteNames),
                IsOrdinaryAdmin = isOrdinaryAdmin,
                RoleNames = roleNames
            };
        }
    }
}
