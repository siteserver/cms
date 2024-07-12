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
    public partial class UsersLayerGroupsController : ControllerBase
    {
        private const string Route = "settings/usersLayerGroups";
        private const string RouteAdd = "settings/usersLayerGroups/actions/add";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUsersInGroupsRepository _usersInGroupsRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public UsersLayerGroupsController(
            IAuthManager authManager,
            IUserRepository userRepository,
            IUserGroupRepository userGroupRepository,
            IUsersInGroupsRepository usersInGroupsRepository,
            IDepartmentRepository departmentRepository
        )
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _usersInGroupsRepository = usersInGroupsRepository;
            _departmentRepository = departmentRepository;
        }

        public class GetRequest
        {
            public string UserIds { get; set; }
        }

        public class GetResult
        {
            public List<User> Users { get; set; }
            public List<UserGroup> Groups { get; set; }
        }

        public class SubmitRequest
        {
            public string UserIds { get; set; }
            public List<int> GroupIds { get; set; }
            public bool IsCancel { get; set; }
        }

        public class AddRequest
        {
            public string UserIds { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }
    }
}
