using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Tag
    {
        private static readonly object LockObject = new object();

        public static List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int publishmentSystemId,
            string guid)
        {
            lock (LockObject)
            {
                var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Tag),
                    nameof(GetContentIdListByTagCollection), TranslateUtils.ObjectCollectionToString(tagCollection),
                    publishmentSystemId.ToString());
                var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval != null) return retval;

                retval = BaiRongDataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, publishmentSystemId);
                StlCacheUtils.SetCache(cacheKey, retval);
                return retval;
            }
        }
    }
}
