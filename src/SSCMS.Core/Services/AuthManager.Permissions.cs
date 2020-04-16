using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CacheManager.Core;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class AuthManager
    {
        private string _rolesKey;
        private string _appPermissionsKey;
        private string _sitePermissionDictKey;
        private string _channelPermissionDictKey;
        private string _contentPermissionDictKey;

        private IList<string> _roles;
        private List<string> _appPermissions;
        private Dictionary<int, List<string>> _sitePermissionDict;
        private Dictionary<string, List<string>> _channelPermissionDict;
        private Dictionary<string, List<string>> _contentPermissionDict;

        public async Task<bool> IsSuperAdminAsync()
        {
            return _databaseManager.RoleRepository.IsConsoleAdministrator(await GetRolesAsync());
        }

        public async Task<bool> IsSiteAdminAsync()
        {
            return _databaseManager.RoleRepository.IsSystemAdministrator(await GetRolesAsync());
        }

        public async Task<bool> IsSiteAdminAsync(int siteId)
        {
            var siteIdList = await GetSiteIdsAsync();
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

        public async Task<List<int>> GetSiteIdsAsync()
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
                var dict = await GetSitePermissionDictAsync();

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

        public async Task<List<int>> GetChannelIdsAsync(int siteId, params string[] permissions)
        {
            if (await IsSiteAdminAsync(siteId))
            {
                return await _databaseManager.ChannelRepository.GetChannelIdListAsync(siteId);
            }

            var siteChannelIdList = new List<int>();
            var dict = await GetChannelPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParsePermissionDictKey(dictKey);
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

        public async Task<bool> HasAppPermissionsAsync(params string[] permissions)
        {
            if (await IsSiteAdminAsync()) return true;

            var permissionList = await GetAppPermissionsAsync();
            return permissions.Any(permission => permissionList.Contains(permission));
        }

        public async Task<List<string>> GetAppPermissionsAsync()
        {
            if (_appPermissions != null) return _appPermissions;

            var administrator = await GetAdminAsync();
            if (administrator == null || administrator.Locked) return new List<string>();

            _appPermissions = _cacheManager.Get<List<string>>(_appPermissionsKey);

            if (_appPermissions == null)
            {
                var roles = await GetRolesAsync();
                if (_databaseManager.RoleRepository.IsConsoleAdministrator(roles))
                {
                    _appPermissions = new List<string>();

                    foreach (var permission in _permissionsAccessor.CurrentValue.App)
                    {
                        _appPermissions.Add(permission.Id);
                    }
                }
                else if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
                {
                    _appPermissions = new List<string>
                    {
                        Constants.AppPermissions.SettingsAdministrators
                    };
                }
                else
                {
                    _appPermissions = await _databaseManager.PermissionsInRolesRepository.GetGeneralPermissionListAsync(roles);
                }

                var cacheItem = new CacheItem<object>(_appPermissionsKey, _appPermissions, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _appPermissions);
            }
            return _appPermissions ??= new List<string>();
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId)
        {
            var dict = await GetSitePermissionDictAsync();
            return await IsSiteAdminAsync() || dict.ContainsKey(siteId);
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions)
        {
            if (await IsSiteAdminAsync()) return true;
            var dict = await GetSitePermissionDictAsync();
            if (!dict.ContainsKey(siteId)) return false;

            var websitePermissionList = dict[siteId];
            if (websitePermissionList != null && websitePermissionList.Count > 0)
            {
                return permissions.Any(sitePermission => websitePermissionList.Contains(sitePermission));
            }

            return false;
        }

        public async Task<List<string>> GetSitePermissionsAsync(int siteId)
        {
            var dict = await GetSitePermissionDictAsync();
            return dict.TryGetValue(siteId, out var list) ? list : new List<string>();
        }

        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId, params string[] permissions)
        {
            while (true)
            {
                if (channelId == 0) return false;
                if (await IsSiteAdminAsync()) return true;
                var dictKey = GetPermissionDictKey(siteId, channelId);
                var dict = await GetChannelPermissionDictAsync();
                if (dict.ContainsKey(dictKey) && await HasPermissionsAsync(dict[dictKey], permissions)) return true;

                var parentChannelId = await _databaseManager.ChannelRepository.GetParentIdAsync(siteId, channelId);
                channelId = parentChannelId;
            }
        }
        
        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId)
        {
            if (channelId == 0) return false;
            if (await IsSiteAdminAsync(siteId))
            {
                return true;
            }
            var dictKey = GetPermissionDictKey(siteId, channelId);
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
            var dictKey = GetPermissionDictKey(siteId, channelId);
            var dict = await GetChannelPermissionDictAsync();
            return dict.TryGetValue(dictKey, out var list) ? list : new List<string>();
        }

        public async Task<List<string>> GetChannelPermissionsAsync(int siteId)
        {
            var list = new List<string>();
            var dict = await GetChannelPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParsePermissionDictKey(dictKey);
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

        //
        public async Task<bool> HasContentPermissionsAsync(int siteId, int channelId, params string[] permissions)
        {
            while (true)
            {
                if (channelId == 0) return false;
                if (await IsSiteAdminAsync()) return true;
                var dictKey = GetPermissionDictKey(siteId, channelId);
                var dict = await GetContentPermissionDictAsync();
                if (dict.ContainsKey(dictKey) && await HasPermissionsAsync(dict[dictKey], permissions)) return true;

                var parentChannelId = await _databaseManager.ChannelRepository.GetParentIdAsync(siteId, channelId);
                channelId = parentChannelId;
            }
        }

        public async Task<bool> HasContentPermissionsAsync(int siteId, int channelId)
        {
            if (channelId == 0) return false;
            if (await IsSiteAdminAsync(siteId))
            {
                return true;
            }
            var dictKey = GetPermissionDictKey(siteId, channelId);
            var dict = await GetContentPermissionDictAsync();
            if (dict.ContainsKey(dictKey))
            {
                return true;
            }

            var parentChannelId = await _databaseManager.ChannelRepository.GetParentIdAsync(siteId, channelId);
            return await HasContentPermissionsAsync(siteId, parentChannelId);
        }

        public async Task<List<string>> GetContentPermissionsAsync(int siteId, int channelId)
        {
            var dictKey = GetPermissionDictKey(siteId, channelId);
            var dict = await GetContentPermissionDictAsync();
            return dict.TryGetValue(dictKey, out var list) ? list : new List<string>();
        }

        public async Task<List<string>> GetContentPermissionsAsync(int siteId)
        {
            var list = new List<string>();
            var dict = await GetContentPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParsePermissionDictKey(dictKey);
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
        //

        private async Task<IList<string>> GetRolesAsync()
        {
            if (_roles != null) return _roles;

            var administrator = await GetAdminAsync();
            if (administrator == null || administrator.Locked)
                return new List<string> { PredefinedRole.Administrator.GetValue() };

            _roles = _cacheManager.Get<List<string>>(_rolesKey);
            if (_roles == null)
            {
                _roles = await _databaseManager.AdministratorsInRolesRepository.GetRolesForUserAsync(administrator.UserName);

                var cacheItem = new CacheItem<object>(_rolesKey, _roles, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _roles);
            }

            return _roles ?? new List<string> { PredefinedRole.Administrator.GetValue() };
        }

        private async Task<Dictionary<int, List<string>>> GetSitePermissionDictAsync()
        {
            var administrator = await GetAdminAsync();

            if (_sitePermissionDict != null) return _sitePermissionDict;
            if (administrator == null || administrator.Locked) return new Dictionary<int, List<string>>();

            _sitePermissionDict = _cacheManager.Get<Dictionary<int, List<string>>>(_sitePermissionDictKey);

            if (_sitePermissionDict == null)
            {
                if (await IsSiteAdminAsync())
                {
                    var allWebsitePermissionList = new List<string>();

                    foreach (var permission in _permissionsAccessor.CurrentValue.Site)
                    {
                        allWebsitePermissionList.Add(permission.Id);
                    }

                    var siteIdList = await GetSiteIdsAsync();

                    _sitePermissionDict = new Dictionary<int, List<string>>();
                    foreach (var siteId in siteIdList)
                    {
                        _sitePermissionDict[siteId] = allWebsitePermissionList;
                    }
                }
                else
                {
                    var roles = await GetRolesAsync();
                    _sitePermissionDict = await _databaseManager.SitePermissionsRepository.GetSitePermissionSortedListAsync(roles);
                }

                var cacheItem = new CacheItem<object>(_sitePermissionDictKey, _sitePermissionDict, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _sitePermissionDict);
            }
            return _sitePermissionDict ??= new Dictionary<int, List<string>>();
        }

        private async Task<Dictionary<string, List<string>>> GetChannelPermissionDictAsync()
        {
            var administrator = await GetAdminAsync();

            if (_channelPermissionDict != null) return _channelPermissionDict;
            if (administrator == null || administrator.Locked) return new Dictionary<string, List<string>>();

            _channelPermissionDict = _cacheManager.Get<Dictionary<string, List<string>>>(_channelPermissionDictKey);

            if (_channelPermissionDict == null)
            {
                var roles = await GetRolesAsync();
                if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
                {
                    var allContentPermissionList = new List<string>();

                    foreach (var permission in _permissionsAccessor.CurrentValue.Channel)
                    {
                        allContentPermissionList.Add(permission.Id);
                    }

                    _channelPermissionDict = new Dictionary<string, List<string>>();

                    var siteIdList = await GetSiteIdsAsync();

                    foreach (var siteId in siteIdList)
                    {
                        _channelPermissionDict[GetPermissionDictKey(siteId, siteId)] = allContentPermissionList;
                    }
                }
                else
                {
                    _channelPermissionDict = await _databaseManager.SitePermissionsRepository.GetChannelPermissionSortedListAsync(roles);
                }

                var cacheItem = new CacheItem<object>(_channelPermissionDictKey, _channelPermissionDict, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _channelPermissionDict);
            }

            return _channelPermissionDict ??= new Dictionary<string, List<string>>();
        }

        private async Task<Dictionary<string, List<string>>> GetContentPermissionDictAsync()
        {
            var administrator = await GetAdminAsync();

            if (_contentPermissionDict != null) return _contentPermissionDict;
            if (administrator == null || administrator.Locked) return new Dictionary<string, List<string>>();

            _contentPermissionDict = _cacheManager.Get<Dictionary<string, List<string>>>(_contentPermissionDictKey);

            if (_contentPermissionDict == null)
            {
                var roles = await GetRolesAsync();
                if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
                {
                    var allContentPermissionList = new List<string>();

                    foreach (var permission in _permissionsAccessor.CurrentValue.Channel)
                    {
                        allContentPermissionList.Add(permission.Id);
                    }

                    _contentPermissionDict = new Dictionary<string, List<string>>();

                    var siteIdList = await GetSiteIdsAsync();

                    foreach (var siteId in siteIdList)
                    {
                        _contentPermissionDict[GetPermissionDictKey(siteId, siteId)] = allContentPermissionList;
                    }
                }
                else
                {
                    _contentPermissionDict = await _databaseManager.SitePermissionsRepository.GetContentPermissionSortedListAsync(roles);
                }

                var cacheItem = new CacheItem<object>(_contentPermissionDictKey, _contentPermissionDict, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _contentPermissionDict);
            }

            return _contentPermissionDict ??= new Dictionary<string, List<string>>();
        }

        private async Task<bool> HasPermissionsAsync(List<string> permissionList, params string[] permissions)
        {
            if (await IsSiteAdminAsync())
            {
                return true;
            }
            foreach (var permission in permissions)
            {
                if (permissionList.Contains(permission))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetPermissionDictKey(int siteId, int channelId)
        {
            return $"{siteId}_{channelId}";
        }

        private static KeyValuePair<int, int> ParsePermissionDictKey(string dictKey)
        {
            if (string.IsNullOrEmpty(dictKey) || dictKey.IndexOf("_", StringComparison.Ordinal) == -1) return new KeyValuePair<int, int>(0, 0);

            return new KeyValuePair<int, int>(TranslateUtils.ToInt(dictKey.Split('_')[0]), TranslateUtils.ToInt(dictKey.Split('_')[1]));
        }

        private static string GetRolesCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetRolesCacheKey), userName);
        }

        private static string GetAppPermissionsCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetAppPermissionsCacheKey), userName);
        }

        private static string GetSitePermissionDictCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetSitePermissionDictCacheKey), userName);
        }

        private static string GetChannelPermissionDictCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetChannelPermissionDictCacheKey), userName);
        }

        private static string GetContentPermissionDictCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetContentPermissionDictCacheKey), userName);
        }
    }
}
