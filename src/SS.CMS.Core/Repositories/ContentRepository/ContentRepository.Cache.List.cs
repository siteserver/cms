using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Enumerations;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private readonly object ListLockObject = new object();
        private readonly string ListCachePrefix = DataCacheManager.GetCacheKey(nameof(ContentRepository), "List");

        private string ListGetCacheKey(int channelId, int? onlyAdminId = null)
        {
            return onlyAdminId.HasValue
                ? $"{ListCachePrefix}.{channelId}.{onlyAdminId.Value}"
                : $"{ListCachePrefix}.{channelId}";
        }

        private void ListRemove(int channelId)
        {
            lock (ListLockObject)
            {
                var cacheKey = ListGetCacheKey(channelId);
                DataCacheManager.Remove(cacheKey);
                DataCacheManager.RemoveByPrefix(cacheKey);
            }
        }

        private List<int> ListGetContentIdList(int channelId, int? onlyAdminId)
        {
            lock (ListLockObject)
            {
                var cacheKey = ListGetCacheKey(channelId, onlyAdminId);
                var list = DataCacheManager.Get<List<int>>(cacheKey);
                if (list != null) return list;

                list = new List<int>();
                DataCacheManager.Insert(cacheKey, list);
                return list;
            }
        }

        private void ListAdd(ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (TaxisTypeUtils.Equals(TaxisType.OrderByTaxisDesc, channelInfo.DefaultTaxisType))
            {
                var contentIdList = ListGetContentIdList(channelInfo.Id, null);
                contentIdList.Insert(0, contentInfo.Id);

                contentIdList = ListGetContentIdList(channelInfo.Id, contentInfo.AdminId);
                contentIdList.Insert(0, contentInfo.Id);
            }
            else
            {
                ListRemove(channelInfo.Id);
            }
        }

        private bool ListIsChanged(ChannelInfo channelInfo, ContentInfo contentInfo1, ContentInfo contentInfo2)
        {
            if (contentInfo1.Top != contentInfo2.Top) return true;

            var orderAttributeName =
                TaxisTypeUtils.GetContentOrderAttributeName(
                    TaxisType.Parse(channelInfo.DefaultTaxisType));

            return contentInfo1.Get(orderAttributeName) != contentInfo2.Get(orderAttributeName);
        }

        public List<int> GetContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId, int offset, int limit)
        {
            var list = ListGetContentIdList(channelInfo.Id, onlyAdminId);
            if (list.Count >= offset + limit)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            if (list.Count == offset)
            {
                var dict = ContentGetContentDict(channelInfo.Id);

                var query = GetCacheWhereString(siteInfo, channelInfo, onlyAdminId);
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

            var q = GetCacheWhereString(siteInfo, channelInfo, onlyAdminId);
            QueryOrder(q, channelInfo, string.Empty);

            return GetCacheContentIdList(q, offset, limit);
        }
    }
}