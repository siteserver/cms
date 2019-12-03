using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        private static class ListCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentRepository), nameof(ListCache));

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

            public static void Set(Content content)
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

        public async Task<List<(int ChannelId, int ContentId)>> GetChannelContentIdListAsync(Site site, Channel channel, int adminId, bool isAllContents, int offset, int limit)
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

            var query = Q.Offset(offset).Limit(limit);
            await QueryWhereAsync(query, site, channel, adminId, isAllContents);
            QueryOrder(query, channel, string.Empty, isAllContents);

            if (channelContentIdList.Count == offset)
            {
                var repository = GetRepository(tableName);
                var pageContentList = await GetContentListAsync(repository, query);

                //var pageContentInfoList = await GetContentInfoListAsync(tableName, await GetCacheWhereStringAsync(site, channel, adminId, isAllContents),
                //    GetOrderString(channel, string.Empty, isAllContents), offset, limit);

                foreach (var content in pageContentList)
                {
                    ListCache.Set(content);
                    await SetEntityCacheAsync(repository, content);
                }

                var pageContentIdList = pageContentList.Select(x => (x.ChannelId, x.Id)).ToList();
                channelContentIdList.AddRange(pageContentIdList);
                return pageContentIdList;
            }

            var minList = await GetContentMinListAsync(tableName, query);
            return minList.Select(x => (x.ChannelId, x.Id)).ToList();

            //return GetCacheChannelContentIdList(tableName, await GetCacheWhereStringAsync(site, channel, adminId, isAllContents),
            //    GetOrderString(channel, string.Empty, isAllContents), offset, limit);
        }
    }
}