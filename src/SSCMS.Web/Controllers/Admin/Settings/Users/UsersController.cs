using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersController : ControllerBase
    {
        private const string Route = "settings/users";
        private const string RouteTree = "settings/users/actions/tree";
        private const string RouteDelete = "settings/users/actions/delete";
        private const string RouteImport = "settings/users/actions/import";
        private const string RouteExport = "settings/users/actions/export";
        private const string RouteSet = "settings/users/actions/set";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUsersInGroupsRepository _usersInGroupsRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public UsersController(
            IAuthManager authManager,
            IPathManager pathManager,
            IDatabaseManager databaseManager,
            IUserRepository userRepository,
            IUserGroupRepository userGroupRepository,
            IUsersInGroupsRepository usersInGroupsRepository,
            IDepartmentRepository departmentRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _usersInGroupsRepository = usersInGroupsRepository;
            _departmentRepository  = departmentRepository;
        }

        public class GetRequest
        {
            public int DepartmentId { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public bool? State { get; set; }
            public int GroupId { get; set; }
            public string Order { get; set; }
            public int LastActivityDate { get; set; }
            public string Keyword { get; set; }
        }

        public class GetResult
        {
            public List<User> Users { get; set; }
            public int Total { get; set; }
            public List<UserGroup> Groups { get; set; }
        }

        public class ImportRequest
        {
            public int DepartmentId { get; set; }
        }

        public class ImportResult
        {
            public bool Value { set; get; }
            public int Success { set; get; }
            public int Failure { set; get; }
            public string ErrorMessage { set; get; }
        }

        public class ExportRequest
        {
            public int DepartmentId { get; set; }
        }

        public class SetRequest
        {
            public int UserId { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class TreeResult
        {
            public List<Cascade<int>> Departments { get; set; }
            public int Count { get; set; }
        }
    }
}
