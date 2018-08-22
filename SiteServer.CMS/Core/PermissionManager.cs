using System;
using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
	public class PermissionManager
	{
        public const string CacheKeyPrefix = "SiteServer.CMS.Core.PermissionManager";

        private string[] _roles;
        private List<string> _permissionList;
        private readonly string _rolesKey;
        private readonly string _permissionListKey;

        private Dictionary<int, List<string>> _websitePermissionDict;
        private Dictionary<string, List<string>> _channelPermissionDict;
        private List<string> _channelPermissionListIgnoreChannelId;
        private List<int> _siteIdList;
        private List<int> _owningChannelIdList;

        private readonly string _websitePermissionDictKey;
        private readonly string _channelPermissionDictKey;
        private readonly string _channelPermissionListIgnoreChannelIdKey;
        private readonly string _siteIdListKey;
        private readonly string _owningChannelIdListKey;

        public static PermissionManager GetInstance(string administratorName)
        {
            return new PermissionManager(administratorName);
        }

        private PermissionManager(string userName)
        {
            UserName = string.IsNullOrEmpty(userName) ? AdminManager.AnonymousUserName : userName;

            _rolesKey = GetRolesCacheKey(userName);
            _permissionListKey = GetPermissionListCacheKey(userName);
            _websitePermissionDictKey = GetWebsitePermissionDictCacheKey(userName);
            _channelPermissionDictKey = GetChannelPermissionDictCacheKey(userName);
            _channelPermissionListIgnoreChannelIdKey = GetChannelPermissionListIgnoreChannelIdCacheKey(userName);
            _siteIdListKey = GetSiteIdListCacheKey(userName);
            _owningChannelIdListKey = GetOwningChannelIdListCacheKey(userName);
        }

        public string UserName { get; }

        public bool IsConsoleAdministrator => EPredefinedRoleUtils.IsConsoleAdministrator(Roles);

        public bool IsSystemAdministrator => EPredefinedRoleUtils.IsSystemAdministrator(Roles);

        public bool IsAdministrator => EPredefinedRoleUtils.IsAdministrator(Roles);

        public List<string> PermissionList
        {
            get
            {
                if (_permissionList == null)
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.Equals(UserName, AdminManager.AnonymousUserName))
                    {
                        if (CacheUtils.Get(_permissionListKey) != null)
                        {
                            _permissionList = CacheUtils.Get(_permissionListKey) as List<string>;
                        }
                        else
                        {
                            if (EPredefinedRoleUtils.IsConsoleAdministrator(Roles))
                            {
                                _permissionList = new List<string>();
                                foreach (var permission in PermissionConfigManager.Instance.GeneralPermissions)
                                {
                                    _permissionList.Add(permission.Name);
                                }
                            }
                            else if (EPredefinedRoleUtils.IsSystemAdministrator(Roles))
                            {
                                _permissionList = new List<string>
                                {
                                    ConfigManager.SettingsPermissions.Admin
                                };
                            }
                            else
                            {
                                _permissionList = DataProvider.PermissionsInRolesDao.GetGeneralPermissionList(Roles);
                            }

                            CacheUtils.InsertMinutes(_permissionListKey, _permissionList, 30);
                        }
                    }
                }
                return _permissionList ?? (_permissionList = new List<string>());
            }
        }

        public string[] Roles
        {
            get
            {
                if (_roles == null)
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.Equals(UserName, AdminManager.AnonymousUserName))
                    {
                        if (CacheUtils.Get(_rolesKey) != null)
                        {
                            _roles = (string[])CacheUtils.Get(_rolesKey);
                        }
                        else
                        {
                            _roles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(UserName);
                            CacheUtils.InsertMinutes(_rolesKey, _roles, 30);
                        }
                    }
                }
                if (_roles != null && _roles.Length > 0)
                {
                    return _roles;
                }
                return new[] { EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
            }
        }

        public bool IsInRole(string role)
        {
            foreach (var r in Roles)
            {
                if (role == r) return true;
            }
            return false;
        }

        private Dictionary<int, List<string>> WebsitePermissionDict
        {
            get
            {
                if (_websitePermissionDict == null)
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.Equals(UserName, AdminManager.AnonymousUserName))
                    {
                        if (CacheUtils.Get(_websitePermissionDictKey) != null)
                        {
                            _websitePermissionDict = CacheUtils.Get(_websitePermissionDictKey) as Dictionary<int, List<string>>;
                        }
                        else
                        {
                            if (EPredefinedRoleUtils.IsSystemAdministrator(Roles))
                            {
                                var allWebsitePermissionList = new List<string>();
                                foreach (var permission in PermissionConfigManager.Instance.WebsitePermissions)
                                {
                                    allWebsitePermissionList.Add(permission.Name);
                                }

                                _websitePermissionDict = new Dictionary<int, List<string>>();
                                if (SiteIdList.Count > 0)
                                {
                                    foreach (var siteId in SiteIdList)
                                    {
                                        _websitePermissionDict[siteId] = allWebsitePermissionList;
                                    }
                                }
                            }
                            else
                            {
                                _websitePermissionDict = DataProvider.SitePermissionsDao.GetWebsitePermissionSortedList(Roles);
                            }
                            CacheUtils.InsertMinutes(_websitePermissionDictKey, _websitePermissionDict, 30);
                        }
                    }
                }
                return _websitePermissionDict ?? (_websitePermissionDict = new Dictionary<int, List<string>>());
            }
        }

        public List<int> ChannelPermissionChannelIdList
        {
            get
            {
                var list = new List<int>();
                foreach (var dictKey in ChannelPermissionDict.Keys)
                {
                    var kvp = ParseChannelPermissionDictKey(dictKey);
                    if (!list.Contains(kvp.Value))
                    {
                        list.Add(kvp.Value);
                    }
                }
                return list;
            }
        }

        private Dictionary<string, List<string>> ChannelPermissionDict
        {
            get
            {
                if (_channelPermissionDict == null)
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.Equals(UserName, AdminManager.AnonymousUserName))
                    {
                        if (CacheUtils.Get(_channelPermissionDictKey) != null)
                        {
                            _channelPermissionDict = CacheUtils.Get(_channelPermissionDictKey) as Dictionary<string, List<string>>;
                        }
                        else
                        {
                            if (EPredefinedRoleUtils.IsSystemAdministrator(Roles))
                            {
                                var allChannelPermissionList = new List<string>();
                                foreach (var permission in PermissionConfigManager.Instance.ChannelPermissions)
                                {
                                    allChannelPermissionList.Add(permission.Name);
                                }

                                _channelPermissionDict = new Dictionary<string, List<string>>();

                                if (SiteIdList.Count > 0)
                                {
                                    foreach (var siteId in SiteIdList)
                                    {
                                        _channelPermissionDict[GetChannelPermissionDictKey(siteId, siteId)] = allChannelPermissionList;
                                    }
                                }
                            }
                            else
                            {
                                _channelPermissionDict = DataProvider.SitePermissionsDao.GetChannelPermissionSortedList(Roles);
                            }
                            CacheUtils.InsertMinutes(_channelPermissionDictKey, _channelPermissionDict, 30);
                        }
                    }
                }
                return _channelPermissionDict ?? (_channelPermissionDict = new Dictionary<string, List<string>>());
            }
        }

        public List<string> ChannelPermissionListIgnoreChannelId
        {
            get
            {
                if (_channelPermissionListIgnoreChannelId == null)
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.Equals(UserName, AdminManager.AnonymousUserName))
                    {
                        if (CacheUtils.Get(_channelPermissionListIgnoreChannelIdKey) != null)
                        {
                            _channelPermissionListIgnoreChannelId = CacheUtils.Get(_channelPermissionListIgnoreChannelIdKey) as List<string>;
                        }
                        else
                        {
                            if (EPredefinedRoleUtils.IsSystemAdministrator(Roles))
                            {
                                _channelPermissionListIgnoreChannelId = new List<string>();
                                foreach (var permission in PermissionConfigManager.Instance.ChannelPermissions)
                                {
                                    _channelPermissionListIgnoreChannelId.Add(permission.Name);
                                }
                            }
                            else
                            {
                                _channelPermissionListIgnoreChannelId = DataProvider.SitePermissionsDao.GetChannelPermissionListIgnoreChannelId(Roles);
                            }
                            CacheUtils.InsertMinutes(_channelPermissionListIgnoreChannelIdKey, _channelPermissionListIgnoreChannelId, 30);
                        }
                    }
                }
                return _channelPermissionListIgnoreChannelId ?? (_channelPermissionListIgnoreChannelId = new List<string>());
            }
        }

        public List<int> SiteIdList
        {
            get
            {
                if (_siteIdList != null) return _siteIdList;

                if (CacheUtils.Get(_siteIdListKey) != null)
                {
                    _siteIdList = (List<int>)CacheUtils.Get(_siteIdListKey);
                }
                else
                {
                    if (EPredefinedRoleUtils.IsConsoleAdministrator(Roles))
                    {
                        _siteIdList = SiteManager.GetSiteIdList();
                    }
                    else if (EPredefinedRoleUtils.IsSystemAdministrator(Roles))
                    {
                        var theSiteIdList = DataProvider.AdministratorDao.GetSiteIdList(UserName);
                        _siteIdList = new List<int>();
                        foreach (var siteId in SiteManager.GetSiteIdList())
                        {
                            if (theSiteIdList != null && theSiteIdList.Contains(siteId))
                            {
                                _siteIdList.Add(siteId);
                            }
                        }
                    }
                    else
                    {
                        _siteIdList = new List<int>();
                        foreach (var siteId in WebsitePermissionDict.Keys)
                        {
                            _siteIdList.Add(siteId);
                        }
                    }

                    if (_siteIdList == null)
                    {
                        _siteIdList = new List<int>();
                    }

                    CacheUtils.InsertMinutes(_siteIdListKey, _siteIdList, 30);
                }
                return _siteIdList;
            }
        }

        public List<int> OwningChannelIdList
        {
            get
            {
                if (_owningChannelIdList == null)
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.Equals(UserName, AdminManager.AnonymousUserName))
                    {
                        if (CacheUtils.Get(_owningChannelIdListKey) != null)
                        {
                            _owningChannelIdList = CacheUtils.Get(_owningChannelIdListKey) as List<int>;
                        }
                        else
                        {
                            _owningChannelIdList = new List<int>();

                            if (!IsSystemAdministrator)
                            {
                                foreach (var dictKey in ChannelPermissionDict.Keys)
                                {
                                    var kvp = ParseChannelPermissionDictKey(dictKey);
                                    var channelInfo = ChannelManager.GetChannelInfo(kvp.Key, kvp.Value);
                                    _owningChannelIdList.AddRange(ChannelManager.GetChannelIdList(channelInfo, EScopeType.All, string.Empty, string.Empty, string.Empty));
                                }
                            }

                            CacheUtils.InsertMinutes(_owningChannelIdListKey, _owningChannelIdList, 30);
                        }
                    }
                }
                return _owningChannelIdList ?? (_owningChannelIdList = new List<int>());
            }
        }

        public bool HasSystemPermissions(params string[] permissionArray)
        {
            if (IsSystemAdministrator)
            {
                return true;
            }
            var permissionList = PermissionList;
            foreach (var permission in permissionArray)
            {
                if (permissionList.Contains(permission))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasSitePermissions(int siteId)
        {
            return IsSystemAdministrator || WebsitePermissionDict.ContainsKey(siteId);
        }

        public List<string> GetSitePermissions(int siteId)
        {
            List<string> list;
            if (WebsitePermissionDict.TryGetValue(siteId, out list))
            {
                return list;
            }
            return new List<string>();
        }

        public bool HasSitePermissions(int siteId, params string[] sitePermissions)
        {
            if (IsSystemAdministrator)
            {
                return true;
            }
            if (WebsitePermissionDict.ContainsKey(siteId))
            {
                var websitePermissionList = WebsitePermissionDict[siteId];
                if (websitePermissionList != null && websitePermissionList.Count > 0)
                {
                    foreach (var sitePermission in sitePermissions)
                    {
                        if (websitePermissionList.Contains(sitePermission))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool HasChannelPermissions(List<string> channelPermissionList, params string[] channelPermissions)
        {
            if (IsSystemAdministrator)
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

        public bool HasChannelPermissions(int siteId, int channelId)
        {
            if (channelId == 0) return false;
            if (IsSystemAdministrator)
            {
                return true;
            }
            var dictKey = GetChannelPermissionDictKey(siteId, channelId);
            if (ChannelPermissionDict.ContainsKey(dictKey))
            {
                return true;
            }

            var parentChannelId = ChannelManager.GetParentId(siteId, channelId);
            return HasChannelPermissions(siteId, parentChannelId);
        }

	    public List<string> GetChannelPermissions(int siteId, int channelId)
	    {
	        var dictKey = GetChannelPermissionDictKey(siteId, channelId);
	        List<string> list;
	        if (ChannelPermissionDict.TryGetValue(dictKey, out list))
	        {
	            return list;
	        }
            return new List<string>();
        }

        public List<string> GetChannelPermissions(int siteId)
        {
            var list = new List<string>();
            foreach (var dictKey in ChannelPermissionDict.Keys)
            {
                var kvp = ParseChannelPermissionDictKey(dictKey);
                if (kvp.Key == siteId)
                {
                    foreach (var permission in ChannelPermissionDict[dictKey])
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

        public bool HasChannelPermissions(int siteId, int channelId, params string[] channelPermissions)
        {
            if (channelId == 0) return false;
            if (IsSystemAdministrator)
            {
                return true;
            }
            var dictKey = GetChannelPermissionDictKey(siteId, channelId);
            if (ChannelPermissionDict.ContainsKey(dictKey) && HasChannelPermissions(ChannelPermissionDict[dictKey], channelPermissions))
            {
                return true;
            }

            var parentChannelId = ChannelManager.GetParentId(siteId, channelId);
            return HasChannelPermissions(siteId, parentChannelId, channelPermissions);
        }

        public bool HasChannelPermissionsIgnoreChannelId(params string[] channelPermissions)
        {
            if (IsSystemAdministrator)
            {
                return true;
            }
            if (HasChannelPermissions(ChannelPermissionListIgnoreChannelId, channelPermissions))
            {
                return true;
            }
            return false;
        }

        public bool IsOwningChannelId(int channelId)
        {
            if (IsSystemAdministrator)
            {
                return true;
            }
            if (OwningChannelIdList.Contains(channelId))
            {
                return true;
            }
            return false;
        }

        public bool IsDescendantOwningChannelId(int siteId, int channelId)
        {
            if (IsSystemAdministrator)
            {
                return true;
            }

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
            foreach (var theChannelId in channelIdList)
            {
                if (IsOwningChannelId(theChannelId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsViewContentOnlySelf(int siteId, int channelId)
        {
            if (IsConsoleAdministrator || IsSystemAdministrator)
                return false;
            if (HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentCheck))
                return false;
            return ConfigManager.SystemConfigInfo.IsViewContentOnlySelf;
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
            return $"{CacheKeyPrefix}.{nameof(GetRolesCacheKey)}.{userName}";
        }

        private static string GetPermissionListCacheKey(string userName)
        {
            return $"{CacheKeyPrefix}.{nameof(GetPermissionListCacheKey)}.{userName}";
        }

        private static string GetWebsitePermissionDictCacheKey(string userName)
        {
            return $"{CacheKeyPrefix}.{nameof(GetWebsitePermissionDictCacheKey)}.{userName}";
        }

        private static string GetChannelPermissionDictCacheKey(string userName)
        {
            return $"{CacheKeyPrefix}.{nameof(GetChannelPermissionDictCacheKey)}.{userName}";
        }

        private static string GetChannelPermissionListIgnoreChannelIdCacheKey(string userName)
        {
            return $"{CacheKeyPrefix}.{nameof(GetChannelPermissionListIgnoreChannelIdCacheKey)}.{userName}";
        }

        private static string GetSiteIdListCacheKey(string userName)
        {
            return $"{CacheKeyPrefix}.{nameof(GetSiteIdListCacheKey)}.{userName}";
        }

        private static string GetOwningChannelIdListCacheKey(string userName)
        {
            return $"{CacheKeyPrefix}.{nameof(GetOwningChannelIdListCacheKey)}.{userName}";
        }

        public static void ClearAllCache()
        {
            CacheUtils.RemoveByStartString(CacheKeyPrefix);
        }
    }
}
