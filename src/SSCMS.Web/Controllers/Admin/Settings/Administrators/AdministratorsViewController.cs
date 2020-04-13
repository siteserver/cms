using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
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
    [Authorize(Roles = Constants.RoleTypeAdministrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsViewController : ControllerBase
    {
        private const string Route = "settings/administratorsView";

        private readonly IHttpContextAccessor _context;
        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;

        public AdministratorsViewController(IHttpContextAccessor context, ICacheManager<object> cacheManager, ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, IAdministratorRepository administratorRepository, IUserRepository userRepository, ISiteRepository siteRepository)
        {
            _context = context;
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _administratorRepository = administratorRepository;
            _userRepository = userRepository;
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
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var permissions = new AuthManager(_context, _cacheManager, _settingsManager, _pathManager, _databaseManager, _pluginManager);
            permissions.Init(admin);
            var level = await permissions.GetAdminLevelAsync();
            var isSuperAdmin = await permissions.IsSuperAdminAsync();
            var siteNames = new List<string>();
            if (!isSuperAdmin)
            {
                var siteIdListWithPermissions = await permissions.GetSiteIdListAsync();
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
