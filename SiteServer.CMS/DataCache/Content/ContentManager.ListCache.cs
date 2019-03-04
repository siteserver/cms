using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
        private static class ListCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager), nameof(ListCache));

            private static string GetCacheKey(int channelId, int? onlyAdminId = null)
            {
                return onlyAdminId.HasValue
                    ? $"{CachePrefix}.{channelId}.{onlyAdminId.Value}"
                    : $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                lock(LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    DataCacheManager.Remove(cacheKey);
                    DataCacheManager.RemoveByPrefix(cacheKey);
                }
            }

            public static List<int> GetContentIdList(int channelId, int? onlyAdminId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId, onlyAdminId);
                    var list = DataCacheManager.Get<List<int>>(cacheKey);
                    if (list != null) return list;

                    list = new List<int>();
                    DataCacheManager.Insert(cacheKey, list);
                    return list;
                }
            }

            public static void Add(ChannelInfo channelInfo, ContentInfo contentInfo)
            {
                if (ETaxisTypeUtils.Equals(ETaxisType.OrderByTaxisDesc, channelInfo.Additional.DefaultTaxisType))
                {
                    var contentIdList = GetContentIdList(channelInfo.Id, null);
                    contentIdList.Insert(0, contentInfo.Id);

                    contentIdList = GetContentIdList(channelInfo.Id, contentInfo.AdminId);
                    contentIdList.Insert(0, contentInfo.Id);
                }
                else
                {
                    Remove(channelInfo.Id);
                }
            }

            public static bool IsChanged(ChannelInfo channelInfo, ContentInfo contentInfo1, ContentInfo contentInfo2)
            {
                if (contentInfo1.IsTop != contentInfo2.IsTop) return true;

                var orderAttributeName =
                    ETaxisTypeUtils.GetContentOrderAttributeName(
                        ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType));

                return contentInfo1.Get(orderAttributeName) != contentInfo2.Get(orderAttributeName);
            }
        }

        public static List<int> GetContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId, int offset, int limit)
        {
            var list = ListCache.GetContentIdList(channelInfo.Id, onlyAdminId);
            if (list.Count >= offset + limit)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            if (list.Count == offset)
            {
                var dict = ContentCache.GetContentDict(channelInfo.Id);

                var pageContentInfoList = DataProvider.ContentDao.GetContentInfoList(tableName, DataProvider.ContentDao.GetCacheWhereString(siteInfo, channelInfo, onlyAdminId),
                    DataProvider.ContentDao.GetOrderString(channelInfo, string.Empty), offset, limit);

                foreach (var contentInfo in pageContentInfoList)
                {
                    dict[contentInfo.Id] = contentInfo;
                }

                var pageContentIdList = pageContentInfoList.Select(x => x.Id).ToList();
                list.AddRange(pageContentIdList);
                return pageContentIdList;
            }

            return DataProvider.ContentDao.GetCacheContentIdList(tableName, DataProvider.ContentDao.GetCacheWhereString(siteInfo, channelInfo, onlyAdminId),
                DataProvider.ContentDao.GetOrderString(channelInfo, string.Empty), offset, limit);
        }
    }
}