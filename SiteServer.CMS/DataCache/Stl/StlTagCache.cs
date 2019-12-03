using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.DataCache.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

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
                retVal = await DataProvider.ContentTagRepository.GetContentIdListByTagCollectionAsync(tagCollection, siteId);
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
                retVal = await DataProvider.ContentTagRepository.GetTagListAsync(siteId, contentId, isOrderByCount, totalNum);
                StlCacheManager.Set(cacheKey, retVal);
            }

            return retVal;
        }
    }
}
