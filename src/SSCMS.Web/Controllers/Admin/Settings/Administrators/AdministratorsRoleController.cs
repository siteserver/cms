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
    public partial class AdministratorsRoleController : ControllerBase
    {
        private const string Route = "settings/administratorsRole";
        private const string RouteDelete = "settings/administratorsRole/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IRoleRepository _roleRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;

        public AdministratorsRoleController(
            IAuthManager authManager,
            IRoleRepository roleRepository,
            ISitePermissionsRepository sitePermissionsRepository,
            IPermissionsInRolesRepository permissionsInRolesRepository,
            IAdministratorRepository administratorRepository,
            ISiteRepository siteRepository
        )
        {
            _authManager = authManager;
            _roleRepository = roleRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
        }

        public class ListRequest
        {
            public List<Role> Roles { get; set; }
        }
    }
}
