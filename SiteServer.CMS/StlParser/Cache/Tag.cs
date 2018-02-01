using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Tag
    {
        private static readonly object LockObject = new object();

        public static List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int siteId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Tag),
                       nameof(GetContentIdListByTagCollection), TranslateUtils.ObjectCollectionToString(tagCollection),
                       siteId.ToString());
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, siteId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Tag),
                       nameof(GetTagInfoList), siteId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());
            var retval = StlCacheUtils.GetCache<List<TagInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<TagInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.TagDao.GetTagInfoList(siteId, contentId, isOrderByCount, totalNum);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
