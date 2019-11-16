using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
        private static class ListCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager), nameof(ListCache));

            private static string GetCacheKey(int channelId, int adminId)
            {
                return $"{CachePrefix}.{channelId}.{adminId}";
            }

            public static void Remove(int channelId)
            {
                lock(LockObject)
                {
                    DataCacheManager.RemoveByPrefix($"{CachePrefix}.{channelId}.");
                }
            }

            public static List<int> GetContentIdList(int channelId, int adminId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId, adminId);
                    var list = DataCacheManager.Get<List<int>>(cacheKey);
                    if (list != null) return list;

                    list = new List<int>();
                    DataCacheManager.Insert(cacheKey, list);
                    return list;
                }
            }

            public static void Set(Model.Content content)
            {
                var contentIdList = GetContentIdList(content.ChannelId, 0);
                if (!contentIdList.Contains(content.Id))
                {
                    contentIdList.Add(content.Id);
                }

                contentIdList = GetContentIdList(content.ChannelId, content.AdminId);
                if (!contentIdList.Contains(content.Id))
                {
                    contentIdList.Add(content.Id);
                }
            }
        }

        public static async Task<List<(int ChannelId, int ContentId)>> GetChannelContentIdListAsync(Site site, Channel channel, int adminId, bool isAllContents, int offset, int limit)
        {
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

            var channelContentIdList = new List<(int ChannelId, int ContentId)>();
            foreach (var contentId in ListCache.GetContentIdList(channel.Id, adminId))
            {
                channelContentIdList.Add((channel.Id, contentId));
            }
            if (isAllContents)
            {
                var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.Descendant);
                foreach (var contentChannelId in channelIdList)
                {
                    var contentChannelInfo = await ChannelManager.GetChannelAsync(site.Id, contentChannelId);
                    var channelTableName = await ChannelManager.GetTableNameAsync(site, contentChannelInfo);
                    if (!StringUtils.EqualsIgnoreCase(tableName, channelTableName)) continue;

                    foreach (var contentId in ListCache.GetContentIdList(contentChannelId, adminId))
                    {
                        channelContentIdList.Add((contentChannelId, contentId));
                    }
                }
            }

            if (channelContentIdList.Count >= offset + limit)
            {
                return channelContentIdList.Skip(offset).Take(limit).ToList();
            }

            var query = Q.NewQuery();
            await DataProvider.ContentDao.QueryWhereAsync(query, site, channel, adminId, isAllContents);
            DataProvider.ContentDao.QueryOrder(query, channel, string.Empty, isAllContents);
            query.Offset(offset).Limit(limit);

            if (channelContentIdList.Count == offset)
            {
                var dict = ContentCache.GetContentDict(channel.Id);

                var pageContentList = await DataProvider.ContentDao.GetContentListAsync(tableName, query);

                //var pageContentInfoList = await DataProvider.ContentDao.GetContentInfoListAsync(tableName, await DataProvider.ContentDao.GetCacheWhereStringAsync(site, channel, adminId, isAllContents),
                //    DataProvider.ContentDao.GetOrderString(channel, string.Empty, isAllContents), offset, limit);

                foreach (var content in pageContentList)
                {
                    ListCache.Set(content);
                    dict[content.Id] = content;
                }

                var pageContentIdList = pageContentList.Select(x => (x.ChannelId, x.Id)).ToList();
                channelContentIdList.AddRange(pageContentIdList);
                return pageContentIdList;
            }

            var minList = await DataProvider.ContentDao.GetContentMinListAsync(tableName, query);
            return minList.Select(x => (x.ChannelId, x.Id)).ToList();

            //return DataProvider.ContentDao.GetCacheChannelContentIdList(tableName, await DataProvider.ContentDao.GetCacheWhereStringAsync(site, channel, adminId, isAllContents),
            //    DataProvider.ContentDao.GetOrderString(channel, string.Empty, isAllContents), offset, limit);
        }
    }
}