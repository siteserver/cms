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
    public partial class UsersController : ControllerBase
    {
        private const string Route = "settings/users";
        private const string RouteImport = "settings/users/actions/import";
        private const string RouteExport = "settings/users/actions/export";
        private const string RouteCheck = "settings/users/actions/check";
        private const string RouteLock = "settings/users/actions/lock";
        private const string RouteUnLock = "settings/users/actions/unLock";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UsersController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IUserRepository userRepository, IUserGroupRepository userGroupRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
        }

        public class GetRequest
        {
            public bool? State { get; set; }
            public int GroupId { get; set; }
            public string Order { get; set; }
            public int LastActivityDate { get; set; }
            public string Keyword { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
        }

        public class GetResults
        {
            public List<User> Users { get; set; }
            public int Count { get; set; }
            public List<UserGroup> Groups { get; set; }
        }

        public class ImportResult
        {
            public bool Value { set; get; }
            public int Success { set; get; }
            public int Failure { set; get; }
            public string ErrorMessage { set; get; }
        }
    }
}
