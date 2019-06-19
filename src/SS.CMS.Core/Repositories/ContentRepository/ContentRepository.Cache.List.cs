using System.Collections.Generic;
using System.Linq;
using SS.CMS.Core.Common;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private readonly object ListLockObject = new object();
        private readonly string ListCachePrefix = StringUtils.GetCacheKey(nameof(ContentRepository), "List");

        private string ListGetCacheKey(int channelId, int? onlyUserId = null)
        {
            return onlyUserId.HasValue
                ? $"{ListCachePrefix}.{channelId}.{onlyUserId.Value}"
                : $"{ListCachePrefix}.{channelId}";
        }

        private void ListRemove(int channelId)
        {
            lock (ListLockObject)
            {
                var cacheKey = ListGetCacheKey(channelId);
                _cacheManager.Remove(cacheKey);
                _cacheManager.RemoveByPrefix(cacheKey);
            }
        }

        private List<int> ListGetContentIdList(int channelId, int? onlyUserId)
        {
            lock (ListLockObject)
            {
                var cacheKey = ListGetCacheKey(channelId, onlyUserId);
                var list = _cacheManager.Get<List<int>>(cacheKey);
                if (list != null) return list;

                list = new List<int>();
                _cacheManager.Insert(cacheKey, list);
                return list;
            }
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

        public List<int> GetContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyUserId, int offset, int limit)
        {
            var list = ListGetContentIdList(channelInfo.Id, onlyUserId);
            if (list.Count >= offset + limit)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            if (list.Count == offset)
            {
                var dict = ContentGetContentDict(channelInfo.Id);

                var query = GetCacheWhereString(siteInfo, channelInfo, onlyUserId);
                QueryOrder(query, channelInfo, string.Empty);
                var pageContentInfoList = GetContentInfoList(query, offset, limit);

                foreach (var contentInfo in pageContentInfoList)
                {
                    dict[contentInfo.Id] = contentInfo;
                }

                var pageContentIdList = pageContentInfoList.Select(x => x.Id).ToList();
                list.AddRange(pageContentIdList);
                return pageContentIdList;
            }

            var q = GetCacheWhereString(siteInfo, channelInfo, onlyUserId);
            QueryOrder(q, channelInfo, string.Empty);

            return GetCacheContentIdList(q, offset, limit);
        }
    }
}