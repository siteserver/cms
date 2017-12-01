using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Tag
    {
        private static readonly object LockObject = new object();

        public static List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int publishmentSystemId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Tag),
                       nameof(GetContentIdListByTagCollection), TranslateUtils.ObjectCollectionToString(tagCollection),
                       publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = BaiRongDataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<TagInfo> GetTagInfoList(int publishmentSystemId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Tag),
                       nameof(GetTagInfoList), publishmentSystemId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());
            var retval = StlCacheUtils.GetCache<List<TagInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<TagInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = BaiRongDataProvider.TagDao.GetTagInfoList(publishmentSystemId, contentId, isOrderByCount, totalNum);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
