using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSCMS;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.PluginImpls;
using SSCMS.Utils;

namespace SSCMS.Core.Services.AuthManager
{
    public class AuthManager : IAuthManager
    {
        private readonly HttpContext _context;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly ILogRepository _logRepository;
        private readonly ISiteLogRepository _siteLogRepository;
        private readonly IUserLogRepository _userLogRepository;

        public AuthManager(IHttpContextAccessor context, ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IUserRepository userRepository, IUserGroupRepository userGroupRepository, ILogRepository logRepository, ISiteLogRepository siteLogRepository, IUserLogRepository userLogRepository)
        {
            _context = context.HttpContext;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _logRepository = logRepository;
            _siteLogRepository = siteLogRepository;
            _userLogRepository = userLogRepository;
        }

        private bool _init;
        private string _adminToken;
        private string _userToken;
        private string _apiToken;
        private bool _isAdminAuthenticated;
        private Administrator _administrator;
        private bool _isUserAuthenticated;
        private User _user;
        private UserGroup _userGroup;
        private bool _isApiAuthenticated;
        private IPermissions _permissions;

        private async Task InitAsync()
        {
            if (_init) return;

            if (!string.IsNullOrEmpty(GetAdminToken()))
            {
                var tokenImpl = ParseAccessToken(_adminToken);
                if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                {
                    var admin = await _administratorRepository.GetByUserIdAsync(tokenImpl.UserId);
                    if (admin != null && !admin.Locked && admin.UserName == tokenImpl.UserName)
                    {
                        _isAdminAuthenticated = true;
                        _administrator = admin;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(GetUserToken()))
            {
                var tokenImpl = ParseAccessToken(_userToken);
                if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                {
                    var user = await _userRepository.GetByUserIdAsync(tokenImpl.UserId);
                    if (user != null && !user.Locked && user.Checked && user.UserName == tokenImpl.UserName)
                    {
                        _user = user;
                        _isUserAuthenticated = true;

                        _userGroup = await _userGroupRepository.GetUserGroupAsync(user.GroupId);
                        if (_userGroup != null)
                        {
                            _administrator = await _administratorRepository.GetByUserNameAsync(_userGroup.AdminName);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(GetApiToken()))
            {
                var tokenInfo = await _accessTokenRepository.GetByTokenAsync(_apiToken);
                if (tokenInfo != null)
                {
                    if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                    {
                        var admin = await _administratorRepository.GetByUserNameAsync(tokenInfo.AdminName);
                        if (admin != null && !admin.Locked)
                        {
                            _isAdminAuthenticated = true;
                            _administrator = admin;
                        }
                    }

                    _isApiAuthenticated = true;
                }
            }

            _permissions = new PermissionsImpl(_pathManager, _pluginManager, _databaseManager, _administrator);

            _init = true;
        }

        public string GetApiToken()
        {
            if (!string.IsNullOrEmpty(_apiToken)) return _apiToken;

            _apiToken = string.Empty;

            if (_context.Request.Headers.TryGetValue(Constants.AuthKeyApiHeader, out var headerValue))
            {
                _apiToken = headerValue;
            }
            else if (_context.Request.Query.TryGetValue(Constants.AuthKeyApiQuery, out var queryValue))
            {
                _apiToken = queryValue;
            }
            else if (_context.Request.Cookies.TryGetValue(Constants.AuthKeyApiCookie, out var cookieValue))
            {
                _apiToken = cookieValue;
            }

            if (!string.IsNullOrEmpty(_apiToken) && StringUtils.EndsWith(_apiToken, Constants.EncryptStingIndicator))
            {
                _apiToken = _settingsManager.Decrypt(_apiToken);
            }

            return _apiToken;
        }

        public string GetUserToken()
        {
            if (_userToken != null) return _userToken;
            _userToken = string.Empty;

            if (_context.Request.Cookies.TryGetValue(Constants.AuthKeyUserCookie, out var cookieValue))
            {
                _userToken = cookieValue;
            }
            else if (_context.Request.Headers.TryGetValue(Constants.AuthKeyUserHeader, out var headerValue))
            {
                _userToken = headerValue;
            }
            else if (_context.Request.Query.TryGetValue(Constants.AuthKeyUserQuery, out var queryValue))
            {
                _userToken = queryValue;
            }

            if (!string.IsNullOrEmpty(_userToken) && StringUtils.EndsWith(_userToken, Constants.EncryptStingIndicator))
            {
                _userToken = _settingsManager.Decrypt(_userToken);
            }

            return _userToken;
        }

        public string GetAdminToken()
        {
            if (_adminToken != null) return _adminToken;

            _adminToken = string.Empty;

            if (_context.Request.Cookies.TryGetValue(Constants.AuthKeyAdminCookie, out var cookieValue))
            {
                _adminToken = cookieValue;
            }
            else if (_context.Request.Headers.TryGetValue(Constants.AuthKeyAdminHeader, out var headerValue))
            {
                _adminToken = headerValue;
            }
            else if (_context.Request.Query.TryGetValue(Constants.AuthKeyAdminQuery, out var queryValue))
            {
                _adminToken = queryValue;
            }

            if (!string.IsNullOrEmpty(_adminToken) && StringUtils.EndsWith(_adminToken, Constants.EncryptStingIndicator))
            {
                _adminToken = _settingsManager.Decrypt(_adminToken);
            }

            return _adminToken;
        }

        public async Task<User> GetUserAsync()
        {
            await InitAsync();
            return _user;
        }

        public async Task<Administrator> GetAdminAsync()
        {
            await InitAsync();
            return _administrator;
        }

        public async Task<bool> IsApiAuthenticatedAsync()
        {
            await InitAsync();
            return _isApiAuthenticated;
        }

        public async Task<bool> IsUserAuthenticatedAsync()
        {
            await InitAsync();
            return _isUserAuthenticated;
        }

        public async Task<bool> IsAdminAuthenticatedAsync()
        {
            await InitAsync();
            return _isAdminAuthenticated;
        }

        //public async Task<IPermissions> GetPermissionsAsync()
        //{
        //    await InitAsync();
        //    return _permissions;
        //}

        public async Task<int> GetAdminIdAsync()
        {
            await InitAsync();
            return _administrator?.Id ?? 0;
        }

        public async Task<string> GetAdminNameAsync()
        {
            await InitAsync();

            if (_administrator != null)
            {
                return _administrator.UserName;
            }

            if (_userGroup != null)
            {
                return _userGroup.AdminName;
            }

            return string.Empty;
        }

        public async Task<int> GetUserIdAsync()
        {
            await InitAsync();
            return _user?.Id ?? 0;
        }

        public async Task<string> GetUserNameAsync()
        {
            await InitAsync();
            return _user?.UserName ?? string.Empty;
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
            await InitAsync();
            await _siteLogRepository.AddSiteLogAsync(siteId, channelId, 0, _administrator, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            await InitAsync();
            await _siteLogRepository.AddSiteLogAsync(siteId, channelId, contentId, _administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            await InitAsync();
            await _logRepository.AddAdminLogAsync(_administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action)
        {
            await InitAsync();
            await _logRepository.AddAdminLogAsync(_administrator, action);
        }

        public async Task<string> AdminLoginAsync(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var admin = await _administratorRepository.GetByUserNameAsync(userName);
            if (admin == null || admin.Locked) return null;

            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = GetAccessToken(admin.Id, admin.UserName, expiresAt);

            await _logRepository.AddAdminLogAsync(admin, "管理员登录");

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

        public async Task<string> UserLoginAsync(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null || user.Locked || !user.Checked) return null;

            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = GetAccessToken(user.Id, user.UserName, expiresAt);

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user);
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

        private AccessTokenImpl ParseAccessToken(string accessToken)
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

        public async Task<bool> IsSuperAdminAsync()
        {
            await InitAsync();
            return await _permissions.IsSuperAdminAsync();
        }

        public async Task<bool> IsSiteAdminAsync()
        {
            await InitAsync();
            return await _permissions.IsSuperAdminAsync();
        }

        public async Task<string> GetAdminLevelAsync()
        {
            await InitAsync();
            return await _permissions.GetAdminLevelAsync();
        }

        public async Task<List<int>> GetSiteIdListAsync()
        {
            await InitAsync();
            return await _permissions.GetSiteIdListAsync();
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, params string[] permissions)
        {
            await InitAsync();
            return await _permissions.GetChannelIdListAsync(siteId, permissions);
        }

        public async Task<bool> HasSystemPermissionsAsync(params string[] permissions)
        {
            await InitAsync();
            return await _permissions.HasSystemPermissionsAsync(permissions);
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions)
        {
            await InitAsync();
            return await _permissions.HasSitePermissionsAsync(siteId, permissions);
        }

        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId, params string[] permissions)
        {
            await InitAsync();
            return await _permissions.HasChannelPermissionsAsync(siteId, channelId, permissions);
        }

        public async Task<List<string>> GetPermissionListAsync()
        {
            await InitAsync();
            return await _permissions.GetPermissionListAsync();
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId)
        {
            await InitAsync();
            return await _permissions.HasSitePermissionsAsync(siteId);
        }

        public async Task<List<string>> GetSitePermissionsAsync(int siteId)
        {
            await InitAsync();
            return await _permissions.GetSitePermissionsAsync(siteId);
        }

        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId)
        {
            await InitAsync();
            return await _permissions.HasChannelPermissionsAsync(siteId, channelId);
        }

        public async Task<List<string>> GetChannelPermissionsAsync(int siteId, int channelId)
        {
            await InitAsync();
            return await _permissions.GetChannelPermissionsAsync(siteId, channelId);
        }

        public async Task<List<string>> GetChannelPermissionsAsync(int siteId)
        {
            await InitAsync();
            return await _permissions.GetChannelPermissionsAsync(siteId);
        }
    }
}