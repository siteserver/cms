using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Plugins.Impl;

namespace SS.CMS.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly HttpContext _context;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IAdministratorsInRolesRepository _administratorsInRolesRepository;
        private readonly IPermissionsInRolesRepository _permissionsInRolesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly ILogRepository _logRepository;
        private readonly ISiteLogRepository _siteLogRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly ISitePermissionsRepository _sitePermissionsRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public AuthManager(IHttpContextAccessor context, ISettingsManager settingsManager, IPathManager pathManager, IConfigRepository configRepository, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IAdministratorsInRolesRepository administratorsInRolesRepository, IPermissionsInRolesRepository permissionsInRolesRepository, IUserRepository userRepository, IUserGroupRepository userGroupRepository, ILogRepository logRepository, ISiteLogRepository siteLogRepository, IUserLogRepository userLogRepository, ISitePermissionsRepository sitePermissionsRepository, IRoleRepository roleRepository, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _context = context.HttpContext;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _administratorsInRolesRepository = administratorsInRolesRepository;
            _permissionsInRolesRepository = permissionsInRolesRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _logRepository = logRepository;
            _siteLogRepository = siteLogRepository;
            _userLogRepository = userLogRepository;
            _sitePermissionsRepository = sitePermissionsRepository;
            _roleRepository = roleRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public async Task<IAuthManager> GetApiAsync()
        {
            var apiToken = ApiToken;
            if (!string.IsNullOrEmpty(apiToken))
            {
                var tokenInfo = await _accessTokenRepository.GetByTokenAsync(apiToken);
                if (tokenInfo != null)
                {
                    if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                    {
                        var adminInfo = await _administratorRepository.GetByUserNameAsync(tokenInfo.AdminName);
                        if (adminInfo != null && !adminInfo.Locked)
                        {
                            Administrator = adminInfo;
                            IsAdminLoggin = true;
                        }
                    }

                    IsApiAuthenticated = true;
                }
            }

            return this;
        }

        public async Task<IAuthManager> GetUserAsync()
        {
            var userToken = UserToken;
            if (!string.IsNullOrEmpty(userToken))
            {
                var tokenImpl = ParseAccessToken(userToken);
                if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                {
                    var user = await _userRepository.GetByUserIdAsync(tokenImpl.UserId);
                    if (user != null && !user.Locked && user.Checked && user.UserName == tokenImpl.UserName)
                    {
                        User = user;
                        IsUserLoggin = true;
                    }
                }
            }

            return this;
        }

        public async Task<IAuthManager> GetAdminAsync()
        {
            var adminToken = AdminToken;
            if (!string.IsNullOrEmpty(adminToken))
            {
                var tokenImpl = ParseAccessToken(adminToken);
                if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                {
                    var adminInfo = await _administratorRepository.GetByUserIdAsync(tokenImpl.UserId);
                    if (adminInfo != null && !adminInfo.Locked && adminInfo.UserName == tokenImpl.UserName)
                    {
                        Administrator = adminInfo;
                        IsAdminLoggin = true;
                    }
                }
            }

            return this;
        }

        public bool IsApiAuthenticated { get; private set; }

        public bool IsUserLoggin { get; private set; }

        public bool IsAdminLoggin { get; private set; }

        public string ApiToken
        {
            get
            {
                var accessTokenStr = string.Empty;

                if (_context.Request.Headers.TryGetValue(Constants.AuthKeyApiHeader, out var headerValue))
                {
                    accessTokenStr = headerValue;
                }
                else if (_context.Request.Query.TryGetValue(Constants.AuthKeyApiQuery, out var queryValue))
                {
                    accessTokenStr = queryValue;
                }
                else if (_context.Request.Cookies.TryGetValue(Constants.AuthKeyApiCookie, out var cookieValue))
                {
                    accessTokenStr = cookieValue;
                }

                if (!string.IsNullOrEmpty(accessTokenStr) && StringUtils.EndsWith(accessTokenStr, Constants.EncryptStingIndicator))
                {
                    accessTokenStr = _settingsManager.Decrypt(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        private string UserToken
        {
            get
            {
                var accessTokenStr = string.Empty;

                if (_context.Request.Cookies.TryGetValue(Constants.AuthKeyUserCookie, out var cookieValue))
                {
                    accessTokenStr = cookieValue;
                }
                else if (_context.Request.Headers.TryGetValue(Constants.AuthKeyUserHeader, out var headerValue))
                {
                    accessTokenStr = headerValue;
                }
                else if (_context.Request.Query.TryGetValue(Constants.AuthKeyUserQuery, out var queryValue))
                {
                    accessTokenStr = queryValue;
                }

                if (!string.IsNullOrEmpty(accessTokenStr) && StringUtils.EndsWith(accessTokenStr, Constants.EncryptStingIndicator))
                {
                    accessTokenStr = _settingsManager.Decrypt(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        public string AdminToken
        {
            get
            {
                var accessTokenStr = string.Empty;

                if (_context.Request.Cookies.TryGetValue(Constants.AuthKeyAdminCookie, out var cookieValue))
                {
                    accessTokenStr = cookieValue;
                }
                else if (_context.Request.Headers.TryGetValue(Constants.AuthKeyAdminHeader, out var headerValue))
                {
                    accessTokenStr = headerValue;
                }
                else if (_context.Request.Query.TryGetValue(Constants.AuthKeyAdminQuery, out var queryValue))
                {
                    accessTokenStr = queryValue;
                }

                if (!string.IsNullOrEmpty(accessTokenStr) && StringUtils.EndsWith(accessTokenStr, Constants.EncryptStingIndicator))
                {
                    accessTokenStr = _settingsManager.Decrypt(accessTokenStr);
                }

                return accessTokenStr;
            }
        }

        public async Task AddSiteLogAsync(int siteId, string action)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, string.Empty);
        }

        public async Task AddSiteLogAsync(int siteId, string action, string summary)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, string action, string summary)
        {
            await _siteLogRepository.AddSiteLogAsync(siteId, channelId, 0, Administrator, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            await _siteLogRepository.AddSiteLogAsync(siteId, channelId, contentId, Administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            await _logRepository.AddAdminLogAsync(Administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action)
        {
            await _logRepository.AddAdminLogAsync(Administrator, action);
        }

        private PermissionsImpl _userPermissionsImpl;

        private PermissionsImpl UserPermissionsImpl
        {
            get
            {
                if (_userPermissionsImpl != null) return _userPermissionsImpl;

                if (User != null)
                {
                    var groupInfo = _userGroupRepository.GetUserGroupAsync(User.GroupId).GetAwaiter().GetResult();
                    if (groupInfo != null)
                    {
                        Administrator = _administratorRepository.GetByUserNameAsync(groupInfo.AdminName).GetAwaiter().GetResult();
                    }
                }

                _userPermissionsImpl = new PermissionsImpl(_pathManager, _administratorsInRolesRepository, _permissionsInRolesRepository, _sitePermissionsRepository, _roleRepository, _siteRepository, _channelRepository, Administrator);

                return _userPermissionsImpl;
            }
        }

        public IPermissions UserPermissions => UserPermissionsImpl;

        private PermissionsImpl _adminPermissionsImpl;

        private PermissionsImpl AdminPermissionsImpl
        {
            get
            {
                if (_adminPermissionsImpl != null) return _adminPermissionsImpl;

                _adminPermissionsImpl = new PermissionsImpl(_pathManager, _administratorsInRolesRepository, _permissionsInRolesRepository, _sitePermissionsRepository, _roleRepository, _siteRepository, _channelRepository, Administrator);

                return _adminPermissionsImpl;
            }
        }

        public IPermissions AdminPermissions => AdminPermissionsImpl;

        public int AdminId => Administrator?.Id ?? 0;

        public string AdminName
        {
            get
            {
                if (Administrator != null)
                {
                    return Administrator.UserName;
                }

                if (User != null)
                {
                    var groupInfo = _userGroupRepository.GetUserGroupAsync(User.GroupId).GetAwaiter().GetResult();
                    if (groupInfo != null)
                    {
                        return groupInfo.AdminName;
                    }
                }

                return string.Empty;
            }
        }

        public Administrator Administrator { get; private set; }

        public async Task<string> AdminLoginAsync(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = await _administratorRepository.GetByUserNameAsync(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            Administrator = adminInfo;
            IsAdminLoggin = true;

            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = GetAccessToken(adminInfo.Id, adminInfo.UserName, expiresAt);

            await _logRepository.AddAdminLogAsync(adminInfo, "管理员登录");

            if (isAutoLogin)
            {
                _context.Response.Cookies.Delete(Constants.AuthKeyAdminCookie);
                _context.Response.Cookies.Append(Constants.AuthKeyAdminCookie, accessToken, new CookieOptions
                {
                    Expires = expiresAt
                });
            }
            else
            {
                _context.Response.Cookies.Delete(Constants.AuthKeyAdminCookie);
                _context.Response.Cookies.Append(Constants.AuthKeyAdminCookie, accessToken);
            }

            return accessToken;
        }

        public void AdminLogout()
        {
            _context.Response.Cookies.Delete(Constants.AuthKeyAdminCookie);
        }

        public int UserId => User?.Id ?? 0;

        public string UserName => User?.UserName ?? string.Empty;

        public User User { get; private set; }

        public async Task<string> UserLoginAsync(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null || user.Locked || !user.Checked) return null;

            User = user;

            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = GetAccessToken(UserId, UserName, expiresAt);

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(User);
            await _userLogRepository.AddUserLoginLogAsync(user.Id);

            if (isAutoLogin)
            {
                _context.Response.Cookies.Delete(Constants.AuthKeyUserCookie);
                _context.Response.Cookies.Append(Constants.AuthKeyUserCookie, accessToken, new CookieOptions
                {
                    Expires = expiresAt
                });
            }
            else
            {
                _context.Response.Cookies.Delete(Constants.AuthKeyUserCookie);
                _context.Response.Cookies.Append(Constants.AuthKeyUserCookie, accessToken);
            }

            return accessToken;
        }

        public void UserLogout()
        {
            User = null;
            _context.Response.Cookies.Delete(Constants.AuthKeyUserCookie);
        }

        public string GetAccessToken(int userId, string userName, TimeSpan expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = DateUtils.GetExpiresAt(expiresAt)
            };

            return JsonWebToken.Encode(userToken, _settingsManager.SecurityKey, JwtHashAlgorithm.HS256);
        }

        public string GetAccessToken(int userId, string userName, DateTime expiresAt)
        {
            if (userId <= 0 || string.IsNullOrEmpty(userName)) return null;

            var userToken = new AccessTokenImpl
            {
                UserId = userId,
                UserName = userName,
                ExpiresAt = expiresAt
            };

            return JsonWebToken.Encode(userToken, _settingsManager.SecurityKey, JwtHashAlgorithm.HS256);
        }

        public AccessTokenImpl ParseAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) return new AccessTokenImpl();

            try
            {
                var tokenObj = JsonWebToken.DecodeToObject<AccessTokenImpl>(accessToken, _settingsManager.SecurityKey);

                if (tokenObj?.ExpiresAt.AddDays(Constants.AccessTokenExpireDays) > DateTime.Now)
                {
                    return tokenObj;
                }
            }
            catch
            {
                // ignored
            }

            return new AccessTokenImpl();
        }
    }
}