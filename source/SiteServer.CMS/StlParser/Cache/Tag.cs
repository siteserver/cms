using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Tag
    {
        public static List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Tag), nameof(GetContentIdListByTagCollection), guid, TranslateUtils.ObjectCollectionToString(tagCollection), publishmentSystemId.ToString());
            var retval = Utils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = BaiRongDataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
