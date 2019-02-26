using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Database.Caches.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Caches
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
	            var retval = DataCacheManager.Get<IList<UserGroupInfo>>(CacheKey);
	            if (retval != null) return retval;

	            lock (LockObject)
	            {
	                retval = DataCacheManager.Get<IList<UserGroupInfo>>(CacheKey);
	                if (retval == null)
	                {
	                    retval = DataProvider.UserGroup.GetUserGroupInfoList() ?? new List<UserGroupInfo>();

	                    DataCacheManager.Insert(CacheKey, retval);
	                }
	            }

	            return retval;
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

	    public static UserGroupInfo GetUserGroupInfo(string groupName)
	    {
	        var list = UserGroupManagerCache.GetAllUserGroups();
	        return list.FirstOrDefault(group => group.GroupName == groupName) ?? list[0];
        }

	    public static IList<UserGroupInfo> GetUserGroupInfoList()
	    {
	        return UserGroupManagerCache.GetAllUserGroups();
	    }
    }
}
