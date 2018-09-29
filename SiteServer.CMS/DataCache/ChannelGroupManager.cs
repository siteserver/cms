using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache
{
	public static class ChannelGroupManager
	{
	    private static class ChannelGroupManagerCache
        {
	        private static readonly object LockObject = new object();

	        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(ChannelGroupManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

	        public static Dictionary<int, List<ChannelGroupInfo>> GetAllChannelGroups()
	        {
	            var retval = DataCacheManager.Get<Dictionary<int, List<ChannelGroupInfo>>>(CacheKey);
	            if (retval != null) return retval;

	            lock (LockObject)
	            {
	                retval = DataCacheManager.Get<Dictionary<int, List<ChannelGroupInfo>>>(CacheKey);
	                if (retval == null)
	                {
	                    retval = DataProvider.ChannelGroupDao.GetAllChannelGroups();

	                    DataCacheManager.Insert(CacheKey, retval);
	                }
	            }

	            return retval;
	        }
	    }

	    public static void ClearCache()
	    {
	        ChannelGroupManagerCache.Clear();
	    }

	    public static bool IsExists(int siteId, string groupName)
	    {
	        var list = GetChannelGroupInfoList(siteId);
	        return list.Any(group => group.GroupName == groupName);
	    }

	    public static ChannelGroupInfo GetChannelGroupInfo(int siteId, string groupName)
	    {
	        var list = GetChannelGroupInfoList(siteId);
	        return list.FirstOrDefault(group => group.GroupName == groupName);
	    }

	    public static List<string> GetGroupNameList(int siteId)
	    {
	        var list = GetChannelGroupInfoList(siteId);
	        return list.Select(group => group.GroupName).ToList();
	    }

        public static List<ChannelGroupInfo> GetChannelGroupInfoList(int siteId)
        {
            List<ChannelGroupInfo> list = null;
            var dict = ChannelGroupManagerCache.GetAllChannelGroups();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ChannelGroupInfo>();
        }
    }
}
