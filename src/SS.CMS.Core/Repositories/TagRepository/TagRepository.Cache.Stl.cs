using System.Collections.Generic;
using SS.CMS.Models;
using SS.CMS.Utils;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace SS.CMS.Core.Repositories
{
    public partial class TagRepository
    {
        public async Task<IList<int>> GetContentIdListByTagCollectionAsync(List<string> tagCollection, int siteId)
        {
            var cacheKey = _cache.GetKey(nameof(TagRepository), nameof(GetContentIdListByTagCollectionAsync), TranslateUtils.ObjectCollectionToString(tagCollection), siteId.ToString());

            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await GetContentIdListByTagCollectionToCacheAsync(tagCollection, siteId);
            });
        }

        public async Task<IEnumerable<Tag>> GetTagInfoListAsync(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var cacheKey = _cache.GetKey(nameof(TagRepository), nameof(GetTagInfoList), siteId.ToString(), contentId.ToString(), isOrderByCount.ToString(), totalNum.ToString());

            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await GetTagInfoListToCacheAsync(siteId, contentId, isOrderByCount, totalNum);
            });
        }
    }
}
