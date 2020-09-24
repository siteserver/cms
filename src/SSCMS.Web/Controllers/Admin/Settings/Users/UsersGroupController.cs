using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersGroupController : ControllerBase
    {
        private const string Route = "settings/usersGroup";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UsersGroupController(IAuthManager authManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, IUserGroupRepository userGroupRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _userGroupRepository = userGroupRepository;
        }

        public class GetResult
        {
            public IEnumerable<UserGroup> Groups { get; set; }
            public IEnumerable<string> AdminNames { get; set; }
        }
    }
}
