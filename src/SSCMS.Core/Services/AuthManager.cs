using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class AuthManager : IAuthManager
    {
        private readonly ClaimsPrincipal _principal;
        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;

        public AuthManager(IHttpContextAccessor context, ICacheManager<object> cacheManager, ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager)
        {
            _principal = context.HttpContext.User;
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
        }

        private Administrator _admin;
        private User _user;
        private UserGroup _userGroup;

        public async Task<User> GetUserAsync()
        {
            if (!IsUser) return null;
            if (_user != null) return _user;

            _user = await _databaseManager.UserRepository.GetByUserNameAsync(UserName);
            return _user;
        }

        public void Init(Administrator administrator)
        {
            if (administrator != null && !administrator.Locked)
            {
                _admin = administrator;

                _rolesKey = GetRolesCacheKey(_admin.UserName);
                _permissionListKey = GetPermissionListCacheKey(_admin.UserName);
                _websitePermissionDictKey = GetWebsitePermissionDictCacheKey(_admin.UserName);
                _channelPermissionDictKey = GetChannelPermissionDictCacheKey(_admin.UserName);
            }
        }

        public async Task<Administrator> GetAdminAsync()
        {
            if (_admin != null) return _admin;

            if (IsAdmin)
            {
                _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(AdminName);
            }
            else if (IsUser)
            {
                var user = await GetUserAsync();
                if (user != null && !user.Locked && user.Checked)
                {
                    _userGroup = await _databaseManager.UserGroupRepository.GetUserGroupAsync(user.GroupId);
                    if (_userGroup != null)
                    {
                        _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(_userGroup.AdminName);
                    }
                }

                _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(AdminName);
            }
            else if (IsApi)
            {
                var tokenInfo = await _databaseManager.AccessTokenRepository.GetByTokenAsync(ApiToken);
                if (tokenInfo != null)
                {
                    if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                    {
                        var admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(tokenInfo.AdminName);
                        if (admin != null && !admin.Locked)
                        {
                            _admin = admin;
                        }
                    }
                }
            }

            Init(_admin);

            return _admin;
        }

        public bool IsAdmin => _principal != null && _principal.IsInRole(Constants.RoleTypeAdministrator);

        public int AdminId => IsAdmin
            ? TranslateUtils.ToInt(_principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
            : 0;

        public string AdminName => IsAdmin ? _principal.Identity.Name : string.Empty;

        public bool IsUser => _principal != null && _principal.IsInRole(Constants.RoleTypeUser);

        public int UserId => IsUser
            ? TranslateUtils.ToInt(_principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
            : 0;

        public string UserName => IsUser ? _principal.Identity.Name : string.Empty;

        public bool IsApi => _principal != null && _principal.IsInRole(Constants.RoleTypeApi);

        public string ApiToken => IsApi ? _principal.Identity.Name : string.Empty;

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
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, 0, await GetAdminAsync(), action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, contentId, await GetAdminAsync(), action, summary);
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            await _databaseManager.LogRepository.AddAdminLogAsync(await GetAdminAsync(), action, summary);
        }

        public async Task AddAdminLogAsync(string action)
        {
            await _databaseManager.LogRepository.AddAdminLogAsync(await GetAdminAsync(), action);
        }

        public async Task<bool> IsSuperAdminAsync()
        {
            return _databaseManager.RoleRepository.IsConsoleAdministrator(await GetRolesAsync());
        }

        public async Task<bool> IsSiteAdminAsync()
        {
            return _databaseManager.RoleRepository.IsSystemAdministrator(await GetRolesAsync());
        }

        private async Task<bool> IsSiteAdminAsync(int siteId)
        {
            var siteIdList = await GetSiteIdListAsync();
            return await IsSiteAdminAsync() && siteIdList.Contains(siteId);
        }

        public async Task<string> GetAdminLevelAsync()
        {
            if (await IsSiteAdminAsync())
            {
                return "超级管理员";
            }

            return await IsSiteAdminAsync() ? "站点总管理员" : "普通管理员";
        }

        public async Task<List<int>> GetSiteIdListAsync()
        {
            var administrator = await GetAdminAsync();
            var siteIdList = new List<int>();

            if (await IsSuperAdminAsync())
            {
                siteIdList = (await _databaseManager.SiteRepository.GetSiteIdListAsync()).ToList();
            }
            else if (await IsSiteAdminAsync())
            {
                if (administrator?.SiteIds != null)
                {
                    foreach (var siteId in administrator.SiteIds)
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
            if (await IsSiteAdminAsync()) return true;

            var permissionList = await GetPermissionListAsync();
            return permissions.Any(permission => permissionList.Contains(permission));
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions)
        {
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
            if (_permissionList != null) return _permissionList;

            var administrator = await GetAdminAsync();
            if (administrator == null || administrator.Locked) return new List<string>();

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
            var dict = await GetWebsitePermissionDictAsync();
            return await IsSiteAdminAsync() || dict.ContainsKey(siteId);
        }

        public async Task<List<string>> GetSitePermissionsAsync(int siteId)
        {
            var dict = await GetWebsitePermissionDictAsync();
            return dict.TryGetValue(siteId, out var list) ? list : new List<string>();
        }

        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId)
        {
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
            var dictKey = GetChannelPermissionDictKey(siteId, channelId);
            var dict = await GetChannelPermissionDictAsync();
            return dict.TryGetValue(dictKey, out var list) ? list : new List<string>();
        }

        public async Task<List<string>> GetChannelPermissionsAsync(int siteId)
        {
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