using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache
{
	public static class ContentGroupManager
	{
	    private static class ContentGroupManagerCache
	    {
	        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(ContentGroupManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

	        public static async Task<Dictionary<int, List<ContentGroup>>> GetAllContentGroupsAsync()
	        {
	            var retVal = DataCacheManager.Get<Dictionary<int, List<ContentGroup>>>(CacheKey);
	            if (retVal != null) return retVal;

                retVal = DataCacheManager.Get<Dictionary<int, List<ContentGroup>>>(CacheKey);
                if (retVal == null)
                {
                    retVal = await DataProvider.ContentGroupDao.GetAllContentGroupsAsync();

                    DataCacheManager.Insert(CacheKey, retVal);
                }

                return retVal;
	        }
	    }

	    public static void ClearCache()
	    {
	        ContentGroupManagerCache.Clear();
	    }

	    public static async Task<bool> IsExistsAsync(int siteId, string groupName)
	    {
	        var list = await GetContentGroupListAsync(siteId);
	        return list.Any(group => group.GroupName == groupName);
	    }

	    public static async Task<ContentGroup> GetContentGroupAsync(int siteId, string groupName)
	    {
	        var list = await GetContentGroupListAsync(siteId);
	        return list.FirstOrDefault(group => group.GroupName == groupName);
	    }

	    public static async Task<List<string>> GetGroupNameListAsync(int siteId)
	    {
	        var list = await GetContentGroupListAsync(siteId);
	        return list.Select(group => group.GroupName).ToList();
	    }

        public static async Task<List<ContentGroup>> GetContentGroupListAsync(int siteId)
        {
            List<ContentGroup> list = null;
            var dict = await ContentGroupManagerCache.GetAllContentGroupsAsync();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ContentGroup>();
        }
    }
}
