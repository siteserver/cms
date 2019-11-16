using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache
{
	public static class UserGroupManager
	{
	    private static class UserGroupManagerCache
        {
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(UserGroupManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

	        public static async Task<List<UserGroup>> GetAllUserGroupsAsync()
	        {
	            var retVal = DataCacheManager.Get<List<UserGroup>>(CacheKey);
	            if (retVal != null) return retVal;

                retVal = DataCacheManager.Get<List<UserGroup>>(CacheKey);
                if (retVal == null)
                {
                    retVal = await DataProvider.UserGroupDao.GetUserGroupListAsync() ?? new List<UserGroup>();

                    DataCacheManager.Insert(CacheKey, retVal);
                }

                return retVal;
	        }
	    }

	    public static void ClearCache()
	    {
	        UserGroupManagerCache.Clear();
	    }

	    public static async Task<bool> IsExistsAsync(string groupName)
	    {
	        var list = await UserGroupManagerCache.GetAllUserGroupsAsync();
	        return list.Any(group => group.GroupName == groupName);
	    }

        public static async Task<UserGroup> GetUserGroupAsync(int groupId)
	    {
	        var list = await UserGroupManagerCache.GetAllUserGroupsAsync();
	        return list.FirstOrDefault(group => group.Id == groupId) ?? list[0];
	    }

	    public static async Task<UserGroup> GetUserGroupAsync(string groupName)
	    {
	        var list = await UserGroupManagerCache.GetAllUserGroupsAsync();
	        return list.FirstOrDefault(group => group.GroupName == groupName) ?? list[0];
        }

        public static async Task<string> GetUserGroupNameAsync(int groupId)
        {
            return (await GetUserGroupAsync(groupId)).GroupName;
        }

        public static async Task<List<UserGroup>> GetUserGroupListAsync()
	    {
	        return await UserGroupManagerCache.GetAllUserGroupsAsync();
	    }
    }
}
