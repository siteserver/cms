using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache
{
	public static class ChannelGroupManager
	{
	    private static class ChannelGroupManagerCache
        {
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(ChannelGroupManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

	        public static async Task<Dictionary<int, List<ChannelGroup>>> GetAllChannelGroupsAsync()
	        {
	            var retVal = DataCacheManager.Get<Dictionary<int, List<ChannelGroup>>>(CacheKey);
	            if (retVal != null) return retVal;

                retVal = DataCacheManager.Get<Dictionary<int, List<ChannelGroup>>>(CacheKey);
                if (retVal == null)
                {
                    retVal = await DataProvider.ChannelGroupDao.GetAllChannelGroupsAsync();

                    DataCacheManager.Insert(CacheKey, retVal);
                }

                return retVal;
	        }
	    }

	    public static void ClearCache()
	    {
	        ChannelGroupManagerCache.Clear();
	    }

	    public static async Task<bool> IsExistsAsync(int siteId, string groupName)
	    {
	        var list = await GetChannelGroupListAsync(siteId);
	        return list.Any(group => group.GroupName == groupName);
	    }

	    public static async Task<ChannelGroup> GetChannelGroupAsync(int siteId, string groupName)
	    {
	        var list = await GetChannelGroupListAsync(siteId);
	        return list.FirstOrDefault(group => group.GroupName == groupName);
	    }

	    public static async Task<List<string>> GetGroupNameListAsync(int siteId)
	    {
	        var list = await GetChannelGroupListAsync(siteId);
	        return list.Select(group => group.GroupName).ToList();
	    }

        public static async Task<List<ChannelGroup>> GetChannelGroupListAsync(int siteId)
        {
            List<ChannelGroup> list = null;
            var dict = await ChannelGroupManagerCache.GetAllChannelGroupsAsync();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ChannelGroup>();
        }
    }
}
