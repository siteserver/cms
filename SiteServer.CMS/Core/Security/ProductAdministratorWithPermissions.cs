using SiteServer.Utils;
using System.Collections.Generic;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core.Security
{
	public class ProductAdministratorWithPermissions : AdministratorWithPermissions
	{
		private Dictionary<int, List<string>> _websitePermissionDict;
		private Dictionary<int, List<string>> _channelPermissionDict;
        private List<string> _channelPermissionListIgnoreChannelId;
        private List<int> _siteIdList;
        private List<int> _owningChannelIdList;

        private readonly string _websitePermissionDictKey;
        private readonly string _channelPermissionDictKey;
        private readonly string _channelPermissionListIgnoreChannelIdKey;
        private readonly string _siteIdListKey;
        private readonly string _owningChannelIdListKey;

        public ProductAdministratorWithPermissions(string userName)
            : base(userName)
		{
            _websitePermissionDictKey = PermissionsManager.GetWebsitePermissionDictKey(userName);
            _channelPermissionDictKey = PermissionsManager.GetChannelPermissionDictKey(userName);
            _channelPermissionListIgnoreChannelIdKey = PermissionsManager.GetChannelPermissionListIgnoreChannelIdKey(userName);
            _siteIdListKey = PermissionsManager.GetSiteIdKey(userName);
            _owningChannelIdListKey = PermissionsManager.GetOwningChannelIdListKey(userName);
		}

		public Dictionary<int, List<string>> WebsitePermissionDict
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

		public Dictionary<int, List<string>> ChannelPermissionDict
        {
			get
			{
			    if (_channelPermissionDict == null)
				{
                    if (!string.IsNullOrEmpty(UserName) && !string.Equals(UserName, AdminManager.AnonymousUserName))
                    {
                        if (CacheUtils.Get(_channelPermissionDictKey) != null)
                        {
                            _channelPermissionDict = CacheUtils.Get(_channelPermissionDictKey) as Dictionary<int, List<string>>;
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

                                _channelPermissionDict = new Dictionary<int, List<string>>();

                                if (SiteIdList.Count > 0)
                                {
                                    foreach (var siteId in SiteIdList)
                                    {
                                        _channelPermissionDict[siteId] = allChannelPermissionList;
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
			    return _channelPermissionDict ?? (_channelPermissionDict = new Dictionary<int, List<string>>());
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

        //public string[] Roles
        //{
        //    get
        //    {
        //        if (base.roles == null)
        //        {
        //            if (!string.IsNullOrEmpty(userName) && !string.Equals(userName, AdminManager.AnonymousUserName))
        //            {
        //                if (CacheUtils.Get(base.rolesKey) != null)
        //                {
        //                    base.roles = (string[])CacheUtils.Get(base.rolesKey);
        //                }
        //                else
        //                {
        //                    if (AdminFactory.IsActiveDirectory && StringUtils.EqualsIgnoreCase(userName, AdminFactory.ADAccount))
        //                    {
        //                        base.roles = new string[] { EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator), EPredefinedRoleUtils.GetValue(EPredefinedRole.Administrator) };
        //                    }
        //                    else
        //                    {
        //                        base.roles = RoleManager.GetRolesForUser(userName);
        //                    }
        //                    CacheUtils.Insert(rolesKey, base.roles, 30 * CacheUtils.MinuteFactor, CacheItemPriority.Normal);
        //                }
        //            }
        //        }
        //        if (roles != null && roles.Length > 0)
        //        {
        //            return base.roles;
        //        }
        //        else
        //        {
        //            return new string[]{EPredefinedRoleUtils.GetValue(EPredefinedRole.Everyone)};
        //        }
        //    }
        //}

        //public bool IsInRole(string role)
        //{
        //    foreach (string r in this.Roles)
        //    {
        //        if (role == r) return true;
        //    }
        //    return false;
        //}

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

                            var permissions = PermissionsManager.GetPermissions(UserName);

                            if (!permissions.IsSystemAdministrator)
                            {
                                foreach (var channelId in ProductPermissionsManager.Current.ChannelPermissionDict.Keys)
                                {
                                    _owningChannelIdList.Add(channelId);
                                    _owningChannelIdList.AddRange(DataProvider.ChannelDao.GetIdListForDescendant(channelId));
                                }
                            }

                            CacheUtils.InsertMinutes(_owningChannelIdListKey, _owningChannelIdList, 30);
                        }
                    }
                }
                return _owningChannelIdList ?? (_owningChannelIdList = new List<int>());
            }
        }

        public void ClearCache()
        {
            _websitePermissionDict = null;
            _channelPermissionDict = null;
            _channelPermissionListIgnoreChannelId = null;
            _siteIdList = null;
            _owningChannelIdList = null;

            CacheUtils.Remove(_websitePermissionDictKey);
            CacheUtils.Remove(_channelPermissionDictKey);
            CacheUtils.Remove(_channelPermissionListIgnoreChannelIdKey);
            CacheUtils.Remove(_siteIdListKey);
            CacheUtils.Remove(_owningChannelIdListKey);
        }

        public static ProductAdministratorWithPermissions GetProductAnonymousUserWithPermissions()
        {
            var userWithPermissions = new ProductAdministratorWithPermissions(AdminManager.AnonymousUserName);
            return userWithPermissions;
        }
	}
}
