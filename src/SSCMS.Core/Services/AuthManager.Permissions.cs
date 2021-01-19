using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class AuthManager
    {
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
            if (await IsSuperAdminAsync())
            {
                return "超级管理员";
            }

            return await IsSiteAdminAsync() ? "站点管理员" : "普通管理员";
        }

        public async Task<List<int>> GetSiteIdsAsync()
        {
            var administrator = await GetAdminAsync();
            var siteIdList = new List<int>();

            if (await IsSuperAdminAsync())
            {
                siteIdList = (await _databaseManager.SiteRepository.GetSiteIdsAsync()).ToList();
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

                foreach (var siteId in dict.Keys.Where(siteId => !siteIdList.Contains(siteId)))
                {
                    siteIdList.Add(siteId);
                }
            }

            return siteIdList;
        }

        public async Task<List<int>> GetChannelPermissionsChannelIdsAsync(int siteId)
        {
            if (await IsSiteAdminAsync(siteId))
            {
                return await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId);
            }

            var siteChannelIdList = new List<int>();
            var dict = await GetChannelPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParsePermissionDictKey(dictKey);
                if (kvp.Key == siteId)
                {
                    var theChannelId = kvp.Value;

                    var channelIdList = await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId, theChannelId, ScopeType.All);

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

        public async Task<List<int>> GetContentPermissionsChannelIdsAsync(int siteId)
        {
            if (await IsSiteAdminAsync(siteId))
            {
                return await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId);
            }

            var siteChannelIdList = new List<int>();
            var dict = await GetContentPermissionDictAsync();
            foreach (var dictKey in dict.Keys)
            {
                var kvp = ParsePermissionDictKey(dictKey);
                if (kvp.Key == siteId)
                {
                    var theChannelId = kvp.Value;

                    var channelIdList = await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId, theChannelId, ScopeType.All);

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

        public async Task<List<int>> GetVisibleChannelIdsAsync(List<int> channelIdsWithPermissions)
        {
            var visibleChannelIds = new List<int>();
            foreach (var enabledChannelId in channelIdsWithPermissions)
            {
                var enabledChannel = await _databaseManager.ChannelRepository.GetAsync(enabledChannelId);
                var parentIds = ListUtils.GetIntList(enabledChannel.ParentsPath);
                foreach (var parentId in parentIds.Where(parentId => !visibleChannelIds.Contains(parentId)))
                {
                    visibleChannelIds.Add(parentId);
                }
                if (!visibleChannelIds.Contains(enabledChannelId))
                {
                    visibleChannelIds.Add(enabledChannelId);
                }
            }

            return visibleChannelIds;
        }

        public async Task<List<int>> GetChannelPermissionsChannelIdsAsync(int siteId, params string[] permissions)
        {
            if (await IsSiteAdminAsync(siteId))
            {
                return await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId);
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

        public async Task<List<int>> GetContentPermissionsChannelIdsAsync(int siteId, params string[] permissions)
        {
            if (await IsSiteAdminAsync(siteId))
            {
                return await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId);
            }

            var siteChannelIdList = new List<int>();
            var dict = await GetContentPermissionDictAsync();
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
            var administrator = await GetAdminAsync();
            if (administrator == null || administrator.Locked) return new List<string>();
            var appPermissions = new List<string>();

            var roles = await GetRolesAsync();
            if (_databaseManager.RoleRepository.IsConsoleAdministrator(roles))
            {
                appPermissions.AddRange(_permissions
                    .Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.App))
                    .Select(permission => permission.Id));
            }
            else if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
            {
                appPermissions = new List<string>
                {
                    MenuUtils.AppPermissions.SettingsAdministrators
                };
            }
            else
            {
                appPermissions = await _databaseManager.PermissionsInRolesRepository.GetAppPermissionsAsync(roles);
            }

            return appPermissions;
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

        private async Task<IList<string>> GetRolesAsync()
        {
            var administrator = await GetAdminAsync();
            if (administrator == null || administrator.Locked)
                return new List<string> { PredefinedRole.Administrator.GetValue() };

            return await _databaseManager.AdministratorsInRolesRepository.GetRolesForUserAsync(administrator.UserName);
        }

        private async Task<Dictionary<int, List<string>>> GetSitePermissionDictAsync()
        {
            var administrator = await GetAdminAsync();

            if (administrator == null || administrator.Locked) return new Dictionary<int, List<string>>();

            var sitePermissionDict = new Dictionary<int, List<string>>();

            if (await IsSiteAdminAsync())
            {
                var siteIdList = await GetSiteIdsAsync();
                foreach (var siteId in siteIdList)
                {
                    var site = await _databaseManager.SiteRepository.GetAsync(siteId);
                    var siteType = _settingsManager.GetSiteType(site.SiteType).Id;
                    var sitePermissions = _permissions
                        .Where(x => ListUtils.ContainsIgnoreCase(x.Type, siteType))
                        .Select(permission => permission.Id).ToList();

                    sitePermissionDict[siteId] = sitePermissions;
                }
            }
            else
            {
                var roles = await GetRolesAsync();
                sitePermissionDict = await _databaseManager.SitePermissionsRepository.GetSitePermissionDictionaryAsync(roles);
            }

            return sitePermissionDict;
        }

        private async Task<Dictionary<string, List<string>>> GetChannelPermissionDictAsync()
        {
            var administrator = await GetAdminAsync();

            if (administrator == null || administrator.Locked) return new Dictionary<string, List<string>>();

            var channelPermissionDict = new Dictionary<string, List<string>>();

            var roles = await GetRolesAsync();
            if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
            {
                var allContentPermissionList = _permissions
                    .Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Channel))
                    .Select(permission => permission.Id).ToList();

                var siteIdList = await GetSiteIdsAsync();

                foreach (var siteId in siteIdList)
                {
                    channelPermissionDict[GetPermissionDictKey(siteId, siteId)] = allContentPermissionList;
                }
            }
            else
            {
                channelPermissionDict = await _databaseManager.SitePermissionsRepository.GetChannelPermissionDictionaryAsync(roles);
            }

            return channelPermissionDict;
        }

        private async Task<Dictionary<string, List<string>>> GetContentPermissionDictAsync()
        {
            var administrator = await GetAdminAsync();

            if (administrator == null || administrator.Locked) return new Dictionary<string, List<string>>();

            var contentPermissionDict = new Dictionary<string, List<string>>();

            var roles = await GetRolesAsync();
            if (_databaseManager.RoleRepository.IsSystemAdministrator(roles))
            {
                var allContentPermissionList = _permissions
                    .Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Channel))
                    .Select(permission => permission.Id).ToList();

                var siteIdList = await GetSiteIdsAsync();

                foreach (var siteId in siteIdList)
                {
                    contentPermissionDict[GetPermissionDictKey(siteId, siteId)] = allContentPermissionList;
                }
            }
            else
            {
                contentPermissionDict = await _databaseManager.SitePermissionsRepository.GetContentPermissionDictionaryAsync(roles);
            }

            return contentPermissionDict;
        }

        private async Task<bool> HasPermissionsAsync(IList<string> permissionList, params string[] permissions)
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
    }
}
