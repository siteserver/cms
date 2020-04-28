using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Services;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsLayerViewController : ControllerBase
    {
        private const string Route = "settings/administratorsLayerView";

        private readonly IHttpContextAccessor _context;
        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;

        public AdministratorsLayerViewController(IHttpContextAccessor context, ISettingsManager settingsManager, IPluginManager pluginManager, IAuthManager authManager, IDatabaseManager databaseManager, IAdministratorRepository administratorRepository, ISiteRepository siteRepository)
        {
            _context = context;
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _authManager = authManager;
            _databaseManager = databaseManager;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
        }

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
                !await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var permissions = new AuthManager(_context, _settingsManager, _pluginManager, _databaseManager);
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
