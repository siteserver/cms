using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.PluginImpls
{
    public class PermissionsImpl : IPermissions
    {
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly Administrator _adminInfo;
        private readonly string _rolesKey;
        private readonly string _permissionListKey;
        private readonly string _websitePermissionDictKey;
        private readonly string _channelPermissionDictKey;

        private IList<string> _roles;
        private List<string> _permissionList;
        private Dictionary<int, List<string>> _websitePermissionDict;
        private Dictionary<string, List<string>> _channelPermissionDict;

        public PermissionsImpl(IPathManager pathManager, IPluginManager pluginManager, IDatabaseManager databaseManager, Administrator adminInfo)
        {
            if (adminInfo == null || adminInfo.Locked) return;

            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _databaseManager = databaseManager;
            _adminInfo = adminInfo;

            _rolesKey = GetRolesCacheKey(adminInfo.UserName);
            _permissionListKey = GetPermissionListCacheKey(adminInfo.UserName);
            _websitePermissionDictKey = GetWebsitePermissionDictCacheKey(adminInfo.UserName);
            _channelPermissionDictKey = GetChannelPermissionDictCacheKey(adminInfo.UserName);
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
            var siteIdList = new List<int>();

            if (await IsSuperAdminAsync())
            {
                siteIdList = (await _databaseManager.SiteRepository.GetSiteIdListAsync()).ToList();
            }
            else if (await IsSiteAdminAsync())
            {
                if (_adminInfo?.SiteIds != null)
                {
                    foreach (var siteId in _adminInfo.SiteIds)
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
            if (_adminInfo == null || _adminInfo.Locked) return new List<string>();

            _permissionList = CacheUtils.Get<List<string>>(_permissionListKey);

            if (_permissionList == null)
            {
                var roles = await GetRolesAsync();
                if (_databaseManager.RoleRepository.IsConsoleAdministrator(roles))
                {
                    _permissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync(_pathManager, _pluginManager);
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

                CacheUtils.InsertMinutes(_permissionListKey, _permissionList, 30);
            }
            return _permissionList ??= new List<string>();
        }

        private async Task<IList<string>> GetRolesAsync()
        {
            if (_roles != null) return _roles;
            if (_adminInfo == null || _adminInfo.Locked)
                return new List<string> { PredefinedRole.Administrator.GetValue() };

            _roles = CacheUtils.Get<List<string>>(_rolesKey);
            if (_roles == null)
            {
                _roles = await _databaseManager.AdministratorsInRolesRepository.GetRolesForUserAsync(_adminInfo.UserName);
                CacheUtils.InsertMinutes(_rolesKey, _roles, 30);
            }

            return _roles ?? new List<string> { PredefinedRole.Administrator.GetValue() };
        }

        private async Task<Dictionary<int, List<string>>> GetWebsitePermissionDictAsync()
        {
            if (_websitePermissionDict != null) return _websitePermissionDict;
            if (_adminInfo == null || _adminInfo.Locked) return new Dictionary<int, List<string>>();

            _websitePermissionDict = CacheUtils.Get<Dictionary<int, List<string>>>(_websitePermissionDictKey);

            if (_websitePermissionDict == null)
            {
                if (await IsSiteAdminAsync())
                {
                    var allWebsitePermissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync(_pathManager, _pluginManager);

                    foreach (var permission in instance.WebsitePermissions)
                    {
                        allWebsitePermissionList.Add(permission.Name);
                    }

                    var siteIdList = await GetSiteIdListAsync();

                    _websitePermissionDict = new Dictionary<int, List<string>>();
                    foreach (var siteId in siteIdList)
                    {
                        _websitePermissionDict[siteId] = allWebsitePermissionList;
                    }
                }
                else
                {
                    var roles = await GetRolesAsync();
                    _websitePermissionDict = await _databaseManager.SitePermissionsRepository.GetWebsitePermissionSortedListAsync(roles);
                }
                CacheUtils.InsertMinutes(_websitePermissionDictKey, _websitePermissionDict, 30);
            }
            return _websitePermissionDict ??= new Dictionary<int, List<string>>();
        }

        private async Task<Dictionary<string, List<string>>> GetChannelPermissionDictAsync()
        {
            if (_channelPermissionDict != null) return _channelPermissionDict;
            if (_adminInfo == null || _adminInfo.Locked) return new Dictionary<string, List<string>>();

            _channelPermissionDict = CacheUtils.Get<Dictionary<string, List<string>>>(_channelPermissionDictKey);

            if (_channelPermissionDict == null)
            {
                var roles = await GetRolesAsync();
                if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
                {
                    var allChannelPermissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync(_pathManager, _pluginManager);

                    foreach (var permission in instance.ChannelPermissions)
                    {
                        allChannelPermissionList.Add(permission.Name);
                    }

                    _channelPermissionDict = new Dictionary<string, List<string>>();

                    var siteIdList = await GetSiteIdListAsync();

                    foreach (var siteId in siteIdList)
                    {
                        _channelPermissionDict[GetChannelPermissionDictKey(siteId, siteId)] = allChannelPermissionList;
                    }
                }
                else
                {
                    _channelPermissionDict = await _databaseManager.SitePermissionsRepository.GetChannelPermissionSortedListAsync(roles);
                }
                CacheUtils.InsertMinutes(_channelPermissionDictKey, _channelPermissionDict, 30);
            }

            return _channelPermissionDict ??= new Dictionary<string, List<string>>();
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

        private async Task<bool> HasChannelPermissionsAsync(List<string> channelPermissionList, params string[] channelPermissions)
        {
            if (await IsSiteAdminAsync())
            {
                return true;
            }
            foreach (var channelPermission in channelPermissions)
            {
                if (channelPermissionList.Contains(channelPermission))
                {
                    return true;
                }
            }
            return false;
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

        public static string GetChannelPermissionDictKey(int siteId, int channelId)
        {
            return $"{siteId}_{channelId}";
        }

        private static KeyValuePair<int, int> ParseChannelPermissionDictKey(string dictKey)
        {
            if (string.IsNullOrEmpty(dictKey) || dictKey.IndexOf("_", StringComparison.Ordinal) == -1) return new KeyValuePair<int, int>(0, 0);

            return new KeyValuePair<int, int>(TranslateUtils.ToInt(dictKey.Split('_')[0]), TranslateUtils.ToInt(dictKey.Split('_')[1]));
        }

        private static string GetRolesCacheKey(string userName)
        {
            return CacheUtils.GetCacheKey(nameof(PermissionsImpl), nameof(GetRolesCacheKey), userName);
        }

        private static string GetPermissionListCacheKey(string userName)
        {
            return CacheUtils.GetCacheKey(nameof(PermissionsImpl), nameof(GetPermissionListCacheKey), userName);
        }

        private static string GetWebsitePermissionDictCacheKey(string userName)
        {
            return CacheUtils.GetCacheKey(nameof(PermissionsImpl), nameof(GetWebsitePermissionDictCacheKey), userName);
        }

        private static string GetChannelPermissionDictCacheKey(string userName)
        {
            return CacheUtils.GetCacheKey(nameof(PermissionsImpl), nameof(GetChannelPermissionDictCacheKey), userName);
        }
    }
}
