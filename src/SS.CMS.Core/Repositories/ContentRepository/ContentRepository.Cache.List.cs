using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Services;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private readonly string ListCachePrefix = StringUtils.GetCacheKey(nameof(ContentRepository), "List");

        private string ListGetCacheKey(int channelId, int? onlyUserId = null)
        {
            return onlyUserId.HasValue
                ? $"{ListCachePrefix}.{channelId}.{onlyUserId.Value}"
                : $"{ListCachePrefix}.{channelId}";
        }

        private void ListRemove(int channelId)
        {
            var cacheKey = ListGetCacheKey(channelId);
            CacheManager.Remove(cacheKey);
            CacheManager.RemoveByPrefix(cacheKey);
        }

        private List<int> ListGetContentIdList(int channelId, int? onlyUserId)
        {
            var cacheKey = ListGetCacheKey(channelId, onlyUserId);
            var list = CacheManager.Get<List<int>>(cacheKey);
            if (list != null) return list;

            list = new List<int>();
            CacheManager.Insert(cacheKey, list);
            return list;
        }

        private void ListAdd(ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (TaxisTypeUtils.Equals(TaxisType.OrderByTaxisDesc, channelInfo.DefaultTaxisType))
            {
                var contentIdList = ListGetContentIdList(channelInfo.Id, null);
                contentIdList.Insert(0, contentInfo.Id);

                contentIdList = ListGetContentIdList(channelInfo.Id, contentInfo.UserId);
                contentIdList.Insert(0, contentInfo.Id);
            }
            else
            {
                ListRemove(channelInfo.Id);
            }
        }

        private bool ListIsChanged(ChannelInfo channelInfo, ContentInfo contentInfo1, ContentInfo contentInfo2)
        {
            if (contentInfo1.IsTop != contentInfo2.IsTop) return true;

            var orderAttributeName =
                TaxisTypeUtils.GetContentOrderAttributeName(
                    TaxisType.Parse(channelInfo.DefaultTaxisType));

            return contentInfo1.Get(orderAttributeName) != contentInfo2.Get(orderAttributeName);
        }

        public async Task<IEnumerable<int>> GetContentIdListAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyUserId, int offset, int limit)
        {
            var list = ListGetContentIdList(channelInfo.Id, onlyUserId);
            if (list.Count >= offset + limit)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            if (list.Count == offset)
            {
                var query = GetCacheWhereString(siteInfo, channelInfo, onlyUserId);
                QueryOrder(query, channelInfo, string.Empty);
                var pageContentInfoList = await GetContentInfoListAsync(query, offset, limit);

                var pageContentIdList = pageContentInfoList.Select(x => x.Id).ToList();
                list.AddRange(pageContentIdList);
                return pageContentIdList;
            }

            var q = GetCacheWhereString(siteInfo, channelInfo, onlyUserId);
            QueryOrder(q, channelInfo, string.Empty);

            return await GetCacheContentIdListAsync(q, offset, limit);
        }
    }
}