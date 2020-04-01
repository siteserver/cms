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

        public static List<int> GetContentIdListByTagCollection(List<string> tagCollection, int siteId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetContentIdListByTagCollection), TranslateUtils.ObjectCollectionToString(tagCollection),
                       siteId.ToString());
            var retVal = StlCacheManager.Get<List<int>>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<List<int>>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, siteId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static List<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetTagInfoList), siteId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());
            var retVal = StlCacheManager.Get<List<TagInfo>>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<List<TagInfo>>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.TagDao.GetTagInfoList(siteId, contentId, isOrderByCount, totalNum);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }
    }
}
