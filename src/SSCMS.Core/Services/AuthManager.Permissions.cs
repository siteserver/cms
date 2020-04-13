using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class AuthManager
    {
        private string _rolesKey;
        private string _permissionListKey;
        private string _websitePermissionDictKey;
        private string _channelPermissionDictKey;

        private IList<string> _roles;
        private List<string> _permissionList;
        private Dictionary<int, List<string>> _websitePermissionDict;
        private Dictionary<string, List<string>> _channelPermissionDict;

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

        private async Task<Dictionary<int, List<string>>> GetWebsitePermissionDictAsync()
        {
            var administrator = await GetAdminAsync();

            if (_websitePermissionDict != null) return _websitePermissionDict;
            if (administrator == null || administrator.Locked) return new Dictionary<int, List<string>>();

            _websitePermissionDict = _cacheManager.Get<Dictionary<int, List<string>>>(_websitePermissionDictKey);

            if (_websitePermissionDict == null)
            {
                if (await IsSiteAdminAsync())
                {
                    var allWebsitePermissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync(_cacheManager, _pathManager, _pluginManager);

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

                var cacheItem = new CacheItem<object>(_websitePermissionDictKey, _websitePermissionDict, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _websitePermissionDict);
            }
            return _websitePermissionDict ??= new Dictionary<int, List<string>>();
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
                    var allChannelPermissionList = new List<string>();
                    var instance = await PermissionConfigManager.GetInstanceAsync(_cacheManager, _pathManager, _pluginManager);

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

                var cacheItem = new CacheItem<object>(_channelPermissionDictKey, _channelPermissionDict, ExpirationMode.Sliding, TimeSpan.FromMinutes(30));
                _cacheManager.AddOrUpdate(cacheItem, _ => _channelPermissionDict);
            }

            return _channelPermissionDict ??= new Dictionary<string, List<string>>();
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
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetRolesCacheKey), userName);
        }

        private static string GetPermissionListCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetPermissionListCacheKey), userName);
        }

        private static string GetWebsitePermissionDictCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetWebsitePermissionDictCacheKey), userName);
        }

        private static string GetChannelPermissionDictCacheKey(string userName)
        {
            return CacheUtils.GetClassKey(typeof(AuthManager), nameof(GetChannelPermissionDictCacheKey), userName);
        }
    }
}
