using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using SSCMS.Web.Controllers;

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
        private const int LoginRateLimitWindowMinutes = 10;
        private const int LoginRateLimitMaxAttempts = 10;

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
        private readonly ICacheManager _cacheManager;

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
            IUsersInGroupsRepository usersInGroupsRepository,
            ICacheManager cacheManager
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
            _cacheManager = cacheManager;
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

        private class LoginRateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        public class CreateRequest
        {
            public User User { get; set; }
            public List<string> GroupNames { get; set; }
        }

        private static string GetLoginRateLimitCacheKey(string account, string ipAddress)
        {
            return CacheUtils.GetClassKey(typeof(UsersController), nameof(Login), StringUtils.ToLower(account), ipAddress);
        }

        private bool TryConsumeLoginAttempt(string account, string ipAddress, out int retryAfterSeconds)
        {
            retryAfterSeconds = 0;
            var cacheKey = GetLoginRateLimitCacheKey(account, ipAddress);
            lock (RateLimitLockManager.GetLock(cacheKey))
            {
                var state = _cacheManager.Get<LoginRateLimitState>(cacheKey);
                if (state == null || state.ExpireAt <= DateTime.Now)
                {
                    state = new LoginRateLimitState
                    {
                        Count = 0,
                        ExpireAt = DateTime.Now.AddMinutes(LoginRateLimitWindowMinutes)
                    };
                }

                state.Count++;
                var minutes = Math.Max(1, (int)Math.Ceiling((state.ExpireAt - DateTime.Now).TotalMinutes));
                _cacheManager.AddOrUpdateAbsolute(cacheKey, state, minutes);
                if (state.Count <= LoginRateLimitMaxAttempts) return true;

                retryAfterSeconds = Math.Max(1, (int)Math.Ceiling((state.ExpireAt - DateTime.Now).TotalSeconds));
                return false;
            }
        }

        private void ClearLoginRateLimit(string account, string ipAddress)
        {
            _cacheManager.Remove(GetLoginRateLimitCacheKey(account, ipAddress));
        }
    }
}
