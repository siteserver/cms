using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlTagCache
    {
        public static async Task<List<int>> GetContentIdListByTagCollectionAsync(List<string> tagCollection, int siteId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetContentIdListByTagCollectionAsync), TranslateUtils.ObjectCollectionToString(tagCollection),
                       siteId.ToString());
            var retVal = StlCacheManager.Get<List<int>>(cacheKey);
            if (retVal != null) return retVal;

            retVal = StlCacheManager.Get<List<int>>(cacheKey);
            if (retVal == null)
            {
                retVal = await DataProvider.ContentTagDao.GetContentIdListByTagCollectionAsync(tagCollection, siteId);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }

        public static async Task<List<ContentTag>> GetTagListAsync(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlTagCache),
                       nameof(GetTagListAsync), siteId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());
            var retVal = StlCacheManager.Get<List<ContentTag>>(cacheKey);
            if (retVal != null) return retVal;

            retVal = StlCacheManager.Get<List<ContentTag>>(cacheKey);
            if (retVal == null)
            {
                retVal = await DataProvider.ContentTagDao.GetTagListAsync(siteId, contentId, isOrderByCount, totalNum);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }
    }
}
