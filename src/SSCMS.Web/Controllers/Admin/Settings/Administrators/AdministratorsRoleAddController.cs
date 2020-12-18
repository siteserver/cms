using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsRoleAddController : ControllerBase
    {
        private const string Route = "settings/administratorsRoleAdd";
        private const string RouteSitePermission = "settings/administratorsRoleAdd/actions/sitePermission";

        private readonly ICacheManager _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;

        public AdministratorsRoleAddController(ICacheManager cacheManager, ISettingsManager settingsManager, IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IRoleRepository roleRepository, ISitePermissionsRepository sitePermissionsRepository, IPermissionsInRolesRepository permissionsInRolesRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _roleRepository = roleRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
        }

        public class Option
        {
            public string Name { get; set; }

            public string Text { get; set; }

            public bool Selected { get; set; }
        }

        public class GetRequest
        {
            public int RoleId { get; set; }
        }

        public class GetResult
        {
            public Role Role { get; set; }
            public List<Option> Permissions { get; set; }
            public List<Site> Sites { get; set; }
            public List<SitePermissions> SitePermissions { get; set; }
        }

        public class SubmitRequest
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }
            public List<string> AppPermissions { get; set; }
            public List<SitePermissions> SitePermissions { get; set; }
        }

        public class SitePermissionRequest
        {
            public int RoleId { get; set; }
            public int SiteId { get; set; }
        }

        public class SitePermissionResult
        {
            public Site Site { get; set; }
            public List<Option> SitePermissions { get; set; }
            public List<Option> ChannelPermissions { get; set; }
            public List<Option> ContentPermissions { get; set; }
            public Channel Channel { get; set; }
            public List<int> CheckedChannelIds { get; set; }
        }
    }
}
