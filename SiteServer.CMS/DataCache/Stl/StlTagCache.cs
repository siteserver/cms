using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlTagCache
    {
        private static readonly object LockObject = new object();

        public static List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int siteId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetContentIdListByTagCollection), TranslateUtils.ObjectCollectionToString(tagCollection),
                       siteId.ToString());
            var retval = StlCacheManager.Get<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, siteId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetTagInfoList), siteId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());
            var retval = StlCacheManager.Get<List<TagInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<List<TagInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.TagDao.GetTagInfoList(siteId, contentId, isOrderByCount, totalNum);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
