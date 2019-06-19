using System.Collections.Generic;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class TagRepository
    {
        private readonly object LockObject = new object();

        public IList<int> GetContentIdListByTagCollection(List<string> tagCollection, int siteId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(TagRepository),
                       nameof(GetContentIdListByTagCollection), TranslateUtils.ObjectCollectionToString(tagCollection),
                       siteId.ToString());
            var retval = _cacheManager.Get<IList<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = _cacheManager.Get<IList<int>>(cacheKey);
                if (retval == null)
                {
                    retval = GetContentIdListByTagCollectionToCache(tagCollection, siteId);
                    _cacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public IList<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(TagRepository),
                       nameof(GetTagInfoList), siteId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());
            var retval = _cacheManager.Get<IList<TagInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = _cacheManager.Get<IList<TagInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = GetTagInfoListToCache(siteId, contentId, isOrderByCount, totalNum);
                    _cacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
