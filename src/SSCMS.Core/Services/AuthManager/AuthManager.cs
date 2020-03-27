using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.PluginImpls;
using SSCMS.Utils;

namespace SSCMS.Core.Services.AuthManager
{
    public partial class AuthManager : IAuthManager
    {
        private readonly HttpContext _context;
        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;

        public AuthManager(IHttpContextAccessor context, ICacheManager<object> cacheManager, ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager)
        {
            _context = context.HttpContext;
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
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

        public void Init(Administrator administrator)
        {
            if (administrator != null && !administrator.Locked)
            {
                _isAdminAuthenticated = true;
                _administrator = administrator;

                _rolesKey = GetRolesCacheKey(_administrator.UserName);
                _permissionListKey = GetPermissionListCacheKey(_administrator.UserName);
                _websitePermissionDictKey = GetWebsitePermissionDictCacheKey(_administrator.UserName);
                _channelPermissionDictKey = GetChannelPermissionDictCacheKey(_administrator.UserName);
            }

            _init = true;
        }

        private async Task InitAsync()
        {
            if (_init) return;

            Administrator administrator = null;

            if (!string.IsNullOrEmpty(GetAdminToken()))
            {
                var tokenImpl = ParseAccessToken(_adminToken);
                if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                {
                    var admin = await _databaseManager.AdministratorRepository.GetByUserIdAsync(tokenImpl.UserId);
                    if (admin != null && !admin.Locked && admin.UserName == tokenImpl.UserName)
                    {
                        administrator = admin;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(GetUserToken()))
            {
                var tokenImpl = ParseAccessToken(_userToken);
                if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                {
                    var user = await _databaseManager.UserRepository.GetByUserIdAsync(tokenImpl.UserId);
                    if (user != null && !user.Locked && user.Checked && user.UserName == tokenImpl.UserName)
                    {
                        _user = user;
                        _isUserAuthenticated = true;

                        _userGroup = await _databaseManager.UserGroupRepository.GetUserGroupAsync(user.GroupId);
                        if (_userGroup != null)
                        {
                            administrator = await _databaseManager.AdministratorRepository.GetByUserNameAsync(_userGroup.AdminName);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(GetApiToken()))
            {
                var tokenInfo = await _databaseManager.AccessTokenRepository.GetByTokenAsync(_apiToken);
                if (tokenInfo != null)
                {
                    if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                    {
                        var admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(tokenInfo.AdminName);
                        if (admin != null && !admin.Locked)
                        {
                            administrator = admin;
                        }
                    }

                    _isApiAuthenticated = true;
                }
            }

            Init(administrator);
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
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, 0, _administrator, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            await InitAsync();
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, contentId, _administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            await InitAsync();
            await _databaseManager.LogRepository.AddAdminLogAsync(_administrator, action, summary);
        }

        public async Task AddAdminLogAsync(string action)
        {
            await InitAsync();
            await _databaseManager.LogRepository.AddAdminLogAsync(_administrator, action);
        }

        public async Task<string> AdminLoginAsync(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(userName);
            if (admin == null || admin.Locked) return null;

            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = GetAccessToken(admin.Id, admin.UserName, expiresAt);

            await _databaseManager.LogRepository.AddAdminLogAsync(admin, "管理员登录");

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

            var user = await _databaseManager.UserRepository.GetByUserNameAsync(userName);
            if (user == null || user.Locked || !user.Checked) return null;

            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = GetAccessToken(user.Id, user.UserName, expiresAt);

            await _databaseManager.UserRepository.UpdateLastActivityDateAndCountOfLoginAsync(user);
            await _databaseManager.UserLogRepository.AddUserLoginLogAsync(user.Id);

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
            return _databaseManager.RoleRepository.IsConsoleAdministrator(await GetRolesAsync());
        }

        public async Task<bool> IsSiteAdminAsync()
        {
            await InitAsync();
            return _databaseManager.RoleRepository.IsSystemAdministrator(await GetRolesAsync());
        }

        private async Task<bool> IsSiteAdminAsync(int siteId)
        {
            await InitAsync();
            var siteIdList = await GetSiteIdListAsync();
            return await IsSiteAdminAsync() && siteIdList.Contains(siteId);
        }

        public async Task<string> GetAdminLevelAsync()
        {
            await InitAsync();

            if (await IsSiteAdminAsync())
            {
                return "超级管理员";
            }

            return await IsSiteAdminAsync() ? "站点总管理员" : "普通管理员";
        }

        public async Task<List<int>> GetSiteIdListAsync()
        {
            await InitAsync();

            var siteIdList = new List<int>();

            if (await IsSuperAdminAsync())
            {
                siteIdList = (await _databaseManager.SiteRepository.GetSiteIdListAsync()).ToList();
            }
            else if (await IsSiteAdminAsync())
            {
                if (_administrator?.SiteIds != null)
                {
                    foreach (var siteId in _administrator.SiteIds)
                    {
                        if (!siteIdList.Contains(siteId))
                        {
                            siteIdList.Add(siteId);
                        }
                    }
                }
            }
            else
            {
                var dict = await GetWebsitePermissionDictAsync();

                foreach (var siteId in dict.Keys)
                {
                    if (!siteIdList.Contains(siteId))
                    {
                        siteIdList.Add(siteId);
                    }
                }
            }

            return siteIdList;
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, params string[] permissions)
        {
            await InitAsync();

            if (await IsSiteAdminAsync(siteId))
            {
                return await _databaseManager.ChannelRepository.GetChannelIdListAsync(siteId);
            }

            var siteChannelIdList = new List<int>();
            var dict = await GetChannelPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParseChannelPermissionDictKey(dictKey);
                var dictPermissions = dict[dictKey];
                if (kvp.Key == siteId && dictPermissions.Any(permissions.Contains))
                {
                    var channelInfo = await _databaseManager.ChannelRepository.GetAsync(kvp.Value);

                    var channelIdList = await _databaseManager.ChannelRepository.GetChannelIdsAsync(channelInfo.SiteId, channelInfo.Id, ScopeType.All);

                    foreach (var channelId in channelIdList)
                    {
                        if (!siteChannelIdList.Contains(channelId))
                        {
                            siteChannelIdList.Add(channelId);
                        }
                    }
                }
            }

            return siteChannelIdList;
        }

        public async Task<bool> HasSystemPermissionsAsync(params string[] permissions)
        {
            await InitAsync();

            if (await IsSiteAdminAsync()) return true;

            var permissionList = await GetPermissionListAsync();
            return permissions.Any(permission => permissionList.Contains(permission));
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions)
        {
            await InitAsync();

            if (await IsSiteAdminAsync()) return true;
            var dict = await GetWebsitePermissionDictAsync();
            if (!dict.ContainsKey(siteId)) return false;

            var websitePermissionList = dict[siteId];
            if (websitePermissionList != null && websitePermissionList.Count > 0)
            {
                return permissions.Any(sitePermission => websitePermissionList.Contains(sitePermission));
            }

            return false;
        }

        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId, params string[] permissions)
        {
            await InitAsync();

            while (true)
            {
                if (channelId == 0) return false;
                if (await IsSiteAdminAsync()) return true;
                var dictKey = GetChannelPermissionDictKey(siteId, channelId);
                var dict = await GetChannelPermissionDictAsync();
                if (dict.ContainsKey(dictKey) && await HasChannelPermissionsAsync(dict[dictKey], permissions)) return true;

                var parentChannelId = await _databaseManager.ChannelRepository.GetParentIdAsync(siteId, channelId);
                channelId = parentChannelId;
            }
        }

        public async Task<List<string>> GetPermissionListAsync()
        {
            await InitAsync();

            if (_permissionList != null) return _permissionList;
            if (_administrator == null || _administrator.Locked) return new List<string>();

            _permissionList = _cacheManager.Get<List<string>>(_permissionListKey);

            if (_permissionList == null)
            {
                var roles = await GetRolesAsync();
                if (_databaseManager.RoleRepository.IsConsoleAdministrator(roles))
                {
                    _permissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync(_cacheManager, _pathManager, _pluginManager);
                    foreach (var permission in instance.GeneralPermissions)
                    {
                        _permissionList.Add(permission.Name);
                    }
                }
                else if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
                {
                    _permissionList = new List<string>
                    {
                        Constants.AppPermissions.SettingsAdministrators
                    };
                }
                else
                {
                    _permissionList = await _databaseManager.PermissionsInRolesRepository.GetGeneralPermissionListAsync(roles);
                }

                var cacheItem = new CacheItem<object>(_permissionListKey, _permissionList, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _permissionList);
            }
            return _permissionList ??= new List<string>();
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId)
        {
            await InitAsync();

            var dict = await GetWebsitePermissionDictAsync();
            return await IsSiteAdminAsync() || dict.ContainsKey(siteId);
        }

        public async Task<List<string>> GetSitePermissionsAsync(int siteId)
        {
            await InitAsync();

            var dict = await GetWebsitePermissionDictAsync();
            return dict.TryGetValue(siteId, out var list) ? list : new List<string>();
        }

        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId)
        {
            await InitAsync();

            if (channelId == 0) return false;
            if (await IsSiteAdminAsync(siteId))
            {
                return true;
            }
            var dictKey = GetChannelPermissionDictKey(siteId, channelId);
            var dict = await GetChannelPermissionDictAsync();
            if (dict.ContainsKey(dictKey))
            {
                return true;
            }

            var parentChannelId = await _databaseManager.ChannelRepository.GetParentIdAsync(siteId, channelId);
            return await HasChannelPermissionsAsync(siteId, parentChannelId);
        }

        public async Task<List<string>> GetChannelPermissionsAsync(int siteId, int channelId)
        {
            await InitAsync();

            var dictKey = GetChannelPermissionDictKey(siteId, channelId);
            var dict = await GetChannelPermissionDictAsync();
            return dict.TryGetValue(dictKey, out var list) ? list : new List<string>();
        }

        public async Task<List<string>> GetChannelPermissionsAsync(int siteId)
        {
            await InitAsync();

            var list = new List<string>();
            var dict = await GetChannelPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParseChannelPermissionDictKey(dictKey);
                if (kvp.Key == siteId)
                {
                    foreach (var permission in dict[dictKey])
                    {
                        if (!list.Contains(permission))
                        {
                            list.Add(permission);
                        }
                    }
                }
            }

            return list;
        }
    }
}