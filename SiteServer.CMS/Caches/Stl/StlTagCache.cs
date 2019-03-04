using System.Collections.Generic;
using SiteServer.CMS.Caches.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Caches.Stl
{
    public static class StlTagCache
    {
        private static readonly object LockObject = new object();

        public static IList<int> GetContentIdListByTagCollection(List<string> tagCollection, int siteId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetContentIdListByTagCollection), TranslateUtils.ObjectCollectionToString(tagCollection),
                       siteId.ToString());
            var retval = StlCacheManager.Get<IList<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<IList<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.Tag.GetContentIdListByTagCollection(tagCollection, siteId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static IList<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetTagInfoList), siteId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());
            var retval = StlCacheManager.Get<IList<TagInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<IList<TagInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.Tag.GetTagInfoList(siteId, contentId, isOrderByCount, totalNum);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
