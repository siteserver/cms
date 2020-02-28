using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsView")]
    public partial class AdministratorsViewController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IAdministratorsInRolesRepository _administratorsInRolesRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public AdministratorsViewController(IAuthManager authManager, IPathManager pathManager, IAdministratorRepository administratorRepository, IAdministratorsInRolesRepository administratorsInRolesRepository, IPermissionsInRolesRepository permissionsInRolesRepository, ISitePermissionsRepository sitePermissionsRepository, IRoleRepository roleRepository, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _administratorRepository = administratorRepository;
            _administratorsInRolesRepository = administratorsInRolesRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _roleRepository = roleRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
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

            if (auth.AdminId != admin.Id &&
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var permissions = new PermissionsImpl(_pathManager, _administratorsInRolesRepository, _permissionsInRolesRepository, _sitePermissionsRepository, _roleRepository, _siteRepository, _channelRepository, admin);
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
