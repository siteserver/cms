using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache
{
	public static class ContentTagManager
	{
	    private static class ContentTagManagerCache
	    {
	        private static readonly object LockObject = new object();

	        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(ContentTagManager));

	        public static void Clear()
	        {
	            DataCacheManager.Remove(CacheKey);
	        }

	        public static Dictionary<int, List<ContentTagInfo>> GetAllContentTags()
	        {
	            var retval = DataCacheManager.Get<Dictionary<int, List<ContentTagInfo>>>(CacheKey);
	            if (retval != null) return retval;

	            lock (LockObject)
	            {
	                retval = DataCacheManager.Get<Dictionary<int, List<ContentTagInfo>>>(CacheKey);
	                if (retval == null)
	                {
	                    retval = DataProvider.ContentTagDao.GetAllContentTags();

	                    DataCacheManager.Insert(CacheKey, retval);
	                }
	            }

	            return retval;
	        }
	    }

	    public static void ClearCache()
	    {
	        ContentTagManagerCache.Clear();
	    }

	    public static bool IsExists(int siteId, string groupName)
	    {
	        var list = GetContentTagInfoList(siteId);
	        return list.Any(group => group.TagName == groupName);
	    }

	    public static ContentTagInfo GetContentTagInfo(int siteId, string groupName)
	    {
	        var list = GetContentTagInfoList(siteId);
	        return list.FirstOrDefault(group => group.TagName == groupName);
	    }

	    public static List<string> GetTagNameList(int siteId)
	    {
	        var list = GetContentTagInfoList(siteId);
	        return list.Select(group => group.TagName).ToList();
	    }

        public static List<ContentTagInfo> GetContentTagInfoList(int siteId)
        {
            List<ContentTagInfo> list = null;
            var dict = ContentTagManagerCache.GetAllContentTags();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ContentTagInfo>();
        }
    }
}
