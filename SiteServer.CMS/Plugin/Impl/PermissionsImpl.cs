using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin.Impl
{
    public class PermissionsImpl : IPermissions
    {
        private readonly Administrator _adminInfo;
        private readonly string _rolesKey;
        private readonly string _permissionListKey;
        private readonly string _websitePermissionDictKey;
        private readonly string _channelPermissionDictKey;
        private readonly string _channelPermissionListIgnoreChannelIdKey;
        private readonly string _channelIdListKey;

        private IList<string> _roles;
        private List<string> _permissionList;
        private Dictionary<int, List<string>> _websitePermissionDict;
        private Dictionary<string, List<string>> _channelPermissionDict;
        private List<string> _channelPermissionListIgnoreChannelId;
        private List<int> _channelIdList;

        public PermissionsImpl(Administrator adminInfo)
        {
            if (adminInfo == null || adminInfo.Locked) return;

            _adminInfo = adminInfo;

            _rolesKey = GetRolesCacheKey(adminInfo.UserName);
            _permissionListKey = GetPermissionListCacheKey(adminInfo.UserName);
            _websitePermissionDictKey = GetWebsitePermissionDictCacheKey(adminInfo.UserName);
            _channelPermissionDictKey = GetChannelPermissionDictCacheKey(adminInfo.UserName);
            _channelPermissionListIgnoreChannelIdKey = GetChannelPermissionListIgnoreChannelIdCacheKey(adminInfo.UserName);
            _channelIdListKey = GetChannelIdListCacheKey(adminInfo.UserName);
        }

        public async Task<List<int>> GetChannelIdListAsync()
        {
            if (_channelIdList != null) return _channelIdList;
            if (_adminInfo == null || _adminInfo.Locked) return new List<int>();

            _channelIdList = DataCacheManager.Get<List<int>>(_channelIdListKey);

            if (_channelIdList == null)
            {
                _channelIdList = new List<int>();

                if (!await IsSuperAdminAsync())
                {
                    var dict = await GetChannelPermissionDictAsync();
                    foreach (var dictKey in dict.Keys)
                    {
                        var kvp = ParseChannelPermissionDictKey(dictKey);
                        var channelInfo = await ChannelManager.GetChannelAsync(kvp.Key, kvp.Value);
                        _channelIdList.AddRange(await ChannelManager.GetChannelIdListAsync(channelInfo, EScopeType.All, string.Empty, string.Empty, string.Empty));
                    }
                }

                DataCacheManager.InsertMinutes(_channelIdListKey, _channelIdList, 30);
            }
            return _channelIdList ?? (_channelIdList = new List<int>());
        }

        public async Task<bool> IsSuperAdminAsync()
        {
            return EPredefinedRoleUtils.IsConsoleAdministrator(await GetRolesAsync());
        }

        public async Task<bool> IsSiteAdminAsync()
        {
            return EPredefinedRoleUtils.IsSystemAdministrator(await GetRolesAsync());
        }

        public async Task<bool> IsSiteAdminAsync(int siteId)
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
                siteIdList = await SiteManager.GetSiteIdListAsync();
            }
            else if (await IsSiteAdminAsync())
            {
                if (_adminInfo != null)
                {
                    foreach (var siteId in TranslateUtils.StringCollectionToIntList(_adminInfo.SiteIdCollection))
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
                return await ChannelManager.GetChannelIdListAsync(siteId);
            }

            var siteChannelIdList = new List<int>();
            var dict = await GetChannelPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParseChannelPermissionDictKey(dictKey);
                var dictPermissions = dict[dictKey];
                if (kvp.Key == siteId && dictPermissions.Any(permissions.Contains))
                {
                    var channelInfo = await ChannelManager.GetChannelAsync(kvp.Key, kvp.Value);

                    var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, EScopeType.All);

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

                var parentChannelId = await ChannelManager.GetParentIdAsync(siteId, channelId);
                channelId = parentChannelId;
            }
        }

        //public bool IsSuperAdmin(string userName)
        //{
        //    var adminPermissionsImpl = new PermissionsImpl(userName);
        //    return adminPermissionsImpl.IsConsoleAdministrator;
        //}

        //public bool IsSiteAdmin(string userName, int siteId)
        //{
        //    var adminPermissionsImpl = new PermissionsImpl(userName);
        //    return adminPermissionsImpl.IsSystemAdministrator && adminPermissionsImpl.HasSitePermissions(siteId);
        //}

        public async Task<List<string>> GetPermissionListAsync()
        {
            if (_permissionList != null) return _permissionList;
            if (_adminInfo == null || _adminInfo.Locked) return new List<string>();

            _permissionList = DataCacheManager.Get<List<string>>(_permissionListKey);

            if (_permissionList == null)
            {
                var roles = await GetRolesAsync();
                if (EPredefinedRoleUtils.IsConsoleAdministrator(roles))
                {
                    _permissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync();
                    foreach (var permission in instance.GeneralPermissions)
                    {
                        _permissionList.Add(permission.Name);
                    }
                }
                else if (EPredefinedRoleUtils.IsSystemAdministrator(roles))
                {
                    _permissionList = new List<string>
                    {
                        ConfigManager.SettingsPermissions.Admin
                    };
                }
                else
                {
                    _permissionList = await DataProvider.PermissionsInRolesDao.GetGeneralPermissionListAsync(roles);
                }

                DataCacheManager.InsertMinutes(_permissionListKey, _permissionList, 30);
            }
            return _permissionList ?? (_permissionList = new List<string>());
        }

        private async Task<IList<string>> GetRolesAsync()
        {
            if (_roles != null) return _roles;
            if (_adminInfo == null || _adminInfo.Locked)
                return new List<string> { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };

            _roles = DataCacheManager.Get<List<string>>(_rolesKey);
            if (_roles == null)
            {
                _roles = await DataProvider.AdministratorsInRolesDao.GetRolesForUserAsync(_adminInfo.UserName);
                DataCacheManager.InsertMinutes(_rolesKey, _roles, 30);
            }

            return _roles ?? new List<string> { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
        }

        private async Task<Dictionary<int, List<string>>> GetWebsitePermissionDictAsync()
        {
            if (_websitePermissionDict != null) return _websitePermissionDict;
            if (_adminInfo == null || _adminInfo.Locked) return new Dictionary<int, List<string>>();

            _websitePermissionDict = DataCacheManager.Get<Dictionary<int, List<string>>>(_websitePermissionDictKey);

            if (_websitePermissionDict == null)
            {
                if (await IsSiteAdminAsync())
                {
                    var allWebsitePermissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync();

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
                    _websitePermissionDict = await DataProvider.SitePermissionsDao.GetWebsitePermissionSortedListAsync(roles);
                }
                DataCacheManager.InsertMinutes(_websitePermissionDictKey, _websitePermissionDict, 30);
            }
            return _websitePermissionDict ?? (_websitePermissionDict = new Dictionary<int, List<string>>());
        }

        private async Task<Dictionary<string, List<string>>> GetChannelPermissionDictAsync()
        {
            if (_channelPermissionDict != null) return _channelPermissionDict;
            if (_adminInfo == null || _adminInfo.Locked) return new Dictionary<string, List<string>>();

            _channelPermissionDict = DataCacheManager.Get<Dictionary<string, List<string>>>(_channelPermissionDictKey);

            if (_channelPermissionDict == null)
            {
                var roles = await GetRolesAsync();
                if (EPredefinedRoleUtils.IsSystemAdministrator(roles))
                {
                    var allChannelPermissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync();

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
                    _channelPermissionDict = await DataProvider.SitePermissionsDao.GetChannelPermissionSortedListAsync(roles);
                }
                DataCacheManager.InsertMinutes(_channelPermissionDictKey, _channelPermissionDict, 30);
            }

            return _channelPermissionDict ?? (_channelPermissionDict = new Dictionary<string, List<string>>());
        }

        private async Task<List<string>> GetChannelPermissionListIgnoreChannelIdAsync()
        {
            if (_channelPermissionListIgnoreChannelId != null) return _channelPermissionListIgnoreChannelId;
            if (_adminInfo == null || _adminInfo.Locked) return new List<string>();

            _channelPermissionListIgnoreChannelId =
                DataCacheManager.Get<List<string>>(_channelPermissionListIgnoreChannelIdKey);
            if (_channelPermissionListIgnoreChannelId == null)
            {
                var roles = await GetRolesAsync();
                if (EPredefinedRoleUtils.IsSystemAdministrator(roles))
                {
                    _channelPermissionListIgnoreChannelId = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync();

                    foreach (var permission in instance.ChannelPermissions)
                    {
                        _channelPermissionListIgnoreChannelId.Add(permission.Name);
                    }
                }
                else
                {
                    _channelPermissionListIgnoreChannelId =
                        await DataProvider.SitePermissionsDao.GetChannelPermissionListIgnoreChannelIdAsync(roles);
                }

                DataCacheManager.InsertMinutes(_channelPermissionListIgnoreChannelIdKey,
                    _channelPermissionListIgnoreChannelId, 30);
            }

            return _channelPermissionListIgnoreChannelId ??
                   (_channelPermissionListIgnoreChannelId = new List<string>());
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

            var parentChannelId = await ChannelManager.GetParentIdAsync(siteId, channelId);
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

        public async Task<bool> HasChannelPermissionsIgnoreChannelIdAsync(params string[] channelPermissions)
        {
            if (await IsSiteAdminAsync())
            {
                return true;
            }
            if (await HasChannelPermissionsAsync(await GetChannelPermissionListIgnoreChannelIdAsync(), channelPermissions))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsOwningChannelIdAsync(int channelId)
        {
            if (await IsSiteAdminAsync())
            {
                return true;
            }

            var channelIdList = await GetChannelIdListAsync();
            if (channelIdList.Contains(channelId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsDescendantOwningChannelIdAsync(int siteId, int channelId)
        {
            if (await IsSiteAdminAsync(siteId))
            {
                return true;
            }

            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
            foreach (var theChannelId in channelIdList)
            {
                if (await IsOwningChannelIdAsync(theChannelId))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<int> GetAdminIdAsync(int siteId, int channelId)
        {
            var config = await ConfigManager.GetInstanceAsync();
            if (!config.IsViewContentOnlySelf
                || await IsSuperAdminAsync()
                || await IsSiteAdminAsync()
                || await HasChannelPermissionsAsync(siteId, channelId, ConfigManager.ChannelPermissions.ContentCheck))
            {
                return 0;
            }
            return _adminInfo.Id;
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
            return DataCacheManager.GetCacheKey(nameof(PermissionsImpl), nameof(GetRolesCacheKey), userName);
        }

        private static string GetPermissionListCacheKey(string userName)
        {
            return DataCacheManager.GetCacheKey(nameof(PermissionsImpl), nameof(GetPermissionListCacheKey), userName);
        }

        private static string GetWebsitePermissionDictCacheKey(string userName)
        {
            return DataCacheManager.GetCacheKey(nameof(PermissionsImpl), nameof(GetWebsitePermissionDictCacheKey), userName);
        }

        private static string GetChannelPermissionDictCacheKey(string userName)
        {
            return DataCacheManager.GetCacheKey(nameof(PermissionsImpl), nameof(GetChannelPermissionDictCacheKey), userName);
        }

        private static string GetChannelPermissionListIgnoreChannelIdCacheKey(string userName)
        {
            return DataCacheManager.GetCacheKey(nameof(PermissionsImpl), nameof(GetChannelPermissionListIgnoreChannelIdCacheKey), userName);
        }

        private static string GetChannelIdListCacheKey(string userName)
        {
            return DataCacheManager.GetCacheKey(nameof(PermissionsImpl), nameof(GetChannelIdListCacheKey), userName);
        }

        public static void ClearAllCache()
        {
            DataCacheManager.RemoveByClassName(nameof(PermissionsImpl));
        }
    }
}
