using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public partial class AdministratorsController : ControllerBase
    {
        private const string Route = "administrators";
        private const string RouteActionsLogin = "administrators/actions/login";
        private const string RouteActionsLogout = "administrators/actions/logout";
        private const string RouteActionsResetPassword = "administrators/actions/resetPassword";
        private const string RouteAdministrator = "administrators/{id:int}";
        private const string RouteAdministratorUpdate = "administrators/{id:int}/actions/update";
        private const string RouteAdministratorDelete = "administrators/{id:int}/actions/delete";
        private const int LoginRateLimitWindowMinutes = 10;
        private const int LoginRateLimitMaxAttempts = 10;

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IDbCacheRepository _dbCacheRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStatRepository _statRepository;
        private readonly ICacheManager _cacheManager;

        public AdministratorsController(ISettingsManager settingsManager, IAuthManager authManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IDbCacheRepository dbCacheRepository, ILogRepository logRepository, IStatRepository statRepository, ICacheManager cacheManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _dbCacheRepository = dbCacheRepository;
            _logRepository = logRepository;
            _statRepository = statRepository;
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
            public List<Administrator> Administrators { get; set; }
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
            /// 下次自动登录
            /// </summary>
            public bool IsAutoLogin { get; set; }
        }

        public class LoginResult
        {
            public LoginAdministrator Administrator { get; set; }
            public string AccessToken { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public string SessionId { get; set; }
            public bool IsEnforcePasswordChange { get; set; }
        }

        public class LoginAdministrator
        {
            public int Id { get; set; }
            public string Guid { get; set; }
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string AvatarUrl { get; set; }
            public DateTime? LastActivityDate { get; set; }

            public static LoginAdministrator From(Administrator administrator)
            {
                if (administrator == null) return null;

                return new LoginAdministrator
                {
                    Id = administrator.Id,
                    Guid = administrator.Guid,
                    UserName = administrator.UserName,
                    DisplayName = administrator.DisplayName,
                    AvatarUrl = administrator.AvatarUrl,
                    LastActivityDate = administrator.LastActivityDate
                };
            }
        }

        private class LoginRateLimitState
        {
            public int Count { get; set; }
            public DateTime ExpireAt { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public string NewPassword { get; set; }
        }

        private async Task<bool> IsAdministratorManagementAllowedAsync()
        {
            return await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeAdministrators) &&
                   await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators) &&
                   await _authManager.IsSuperAdminAsync();
        }

        private static string GetLoginRateLimitCacheKey(string account, string ipAddress)
        {
            return CacheUtils.GetClassKey(typeof(AdministratorsController), nameof(Login), StringUtils.ToLower(account), ipAddress);
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
