using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Caches.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Caches
{
	public static class UserGroupManager
	{
	    private static class UserGroupManagerCache
        {
	        private static readonly object LockObject = new object();

	        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(UserGroupManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

	        public static IList<UserGroupInfo> GetAllUserGroups()
	        {
	            var list = DataCacheManager.Get<IList<UserGroupInfo>>(CacheKey);
	            if (list == null)
	            {
	                lock (LockObject)
	                {
	                    list = DataCacheManager.Get<IList<UserGroupInfo>>(CacheKey);
	                    if (list == null)
	                    {
	                        list = DataProvider.UserGroup.GetUserGroupInfoList() ?? new List<UserGroupInfo>();

	                        DataCacheManager.Insert(CacheKey, list);
	                    }
	                }
                }

	            list.Insert(0, new UserGroupInfo
	            {
	                Id = 0,
	                GroupName = "默认用户组",
	                AdminName = ConfigManager.Instance.UserDefaultGroupAdminName
	            });

	            return list;
	        }
	    }

	    public static void ClearCache()
	    {
	        UserGroupManagerCache.Clear();
	    }

	    public static bool IsExists(string groupName)
	    {
	        var list = UserGroupManagerCache.GetAllUserGroups();
	        return list.Any(group => group.GroupName == groupName);
	    }

        public static UserGroupInfo GetUserGroupInfo(int groupId)
	    {
	        var list = UserGroupManagerCache.GetAllUserGroups();
	        return list.FirstOrDefault(group => group.Id == groupId) ?? list[0];
	    }

	    public static IList<UserGroupInfo> GetUserGroupInfoList()
	    {
	        return UserGroupManagerCache.GetAllUserGroups();
	    }
    }
}
