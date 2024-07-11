using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class UsersController : ControllerBase
    {
        private const string Route = "users";
        private const string RouteActionsLogin = "users/actions/login";
        private const string RouteActionsLogout = "users/actions/logout";
        private const string RouteUser = "users/{account}";
        private const string RouteUserAvatar = "users/{id:int}/avatar";
        private const string RouteUserUpdate = "users/{id:int}/actions/update";
        private const string RouteUserDelete = "users/{id:int}/actions/delete";
        private const string RouteUserResetPassword = "users/{id:int}/actions/resetPassword";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStatRepository _statRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUsersInGroupsRepository _usersInGroupsRepository;

        public UsersController(
            IAuthManager authManager,
            IPathManager pathManager,
            IConfigRepository configRepository,
            IAccessTokenRepository accessTokenRepository,
            IUserRepository userRepository,
            ILogRepository logRepository,
            IStatRepository statRepository,
            IDbCacheRepository dbCacheRepository,
            IUserGroupRepository userGroupRepository,
            IUsersInGroupsRepository usersInGroupsRepository
        )
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _statRepository = statRepository;
            _dbCacheRepository = dbCacheRepository;
            _userGroupRepository = userGroupRepository;
            _usersInGroupsRepository = usersInGroupsRepository;
        }

        public class ListRequest
        {
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class ListResult
        {
            public int Count { get; set; }
            public List<User> Users { get; set; }
        }

        public class LoginRequest
        {
            /// <summary>
            /// 账号
            /// </summary>
            public string Account { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// OpenId
            /// </summary>
            public string OpenId { get; set; }

            /// <summary>
            /// 下次自动登录
            /// </summary>
            public bool IsPersistent { get; set; }
        }

        public class LoginResult
        {
            public User User { get; set; }
            public string AccessToken { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }

        public class CreateRequest
        {
            public User User { get; set; }
            public List<string> GroupNames { get; set; }
        }
    }
}
