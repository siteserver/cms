using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache.Core;

namespace SS.CMS.Core.Repositories
{
	public partial class ContentGroupRepository
    {
        private readonly object _lockObject = new object();

        private void ClearCache()
        {
            DataCacheManager.Remove(CacheKey);
        }

        public Dictionary<int, List<ContentGroupInfo>> GetAllContentGroups()
        {
            var retval = DataCacheManager.Get<Dictionary<int, List<ContentGroupInfo>>>(CacheKey);
            if (retval != null) return retval;

            lock (_lockObject)
            {
                retval = DataCacheManager.Get<Dictionary<int, List<ContentGroupInfo>>>(CacheKey);
                if (retval == null)
                {
                    retval = GetAllContentGroupsToCache();

                    DataCacheManager.Insert(CacheKey, retval);
                }
            }

            return retval;
        }

	    public bool IsExists(int siteId, string groupName)
	    {
	        var list = GetContentGroupInfoList(siteId);
	        return list.Any(group => group.GroupName == groupName);
	    }

	    public ContentGroupInfo GetContentGroupInfo(int siteId, string groupName)
	    {
	        var list = GetContentGroupInfoList(siteId);
	        return list.FirstOrDefault(group => group.GroupName == groupName);
	    }

	    public List<string> GetGroupNameList(int siteId)
	    {
	        var list = GetContentGroupInfoList(siteId);
	        return list.Select(group => group.GroupName).ToList();
	    }

        public List<ContentGroupInfo> GetContentGroupInfoList(int siteId)
        {
            List<ContentGroupInfo> list = null;
            var dict = GetAllContentGroups();

            if (dict != null && dict.ContainsKey(siteId))
            {
                list = dict[siteId];
            }
            return list ?? new List<ContentGroupInfo>();
        }
    }
}
