using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;
using SqlKata;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        //public async Task<List<(int ChannelId, int ContentId)>> GetChannelContentIdListAsync(Site site, Channel channel,
        //    int adminId, bool isAllContents, int offset, int limit)
        //{
        //    var tableName = await ChannelManager.GetTableNameAsync(site, channel);

        //    var channelContentIdList = new List<(int ChannelId, int ContentId)>();
        //    foreach (var contentId in ListCache.GetContentIdList(channel.Id, adminId))
        //    {
        //        channelContentIdList.Add((channel.Id, contentId));
        //    }

        //    if (isAllContents)
        //    {
        //        var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.Descendant);
        //        foreach (var contentChannelId in channelIdList)
        //        {
        //            var contentChannelInfo = await ChannelManager.GetChannelAsync(site.Id, contentChannelId);
        //            var channelTableName = await ChannelManager.GetTableNameAsync(site, contentChannelInfo);
        //            if (!StringUtils.EqualsIgnoreCase(tableName, channelTableName)) continue;

        //            foreach (var contentId in ListCache.GetContentIdList(contentChannelId, adminId))
        //            {
        //                channelContentIdList.Add((contentChannelId, contentId));
        //            }
        //        }
        //    }

        //    if (channelContentIdList.Count >= offset + limit)
        //    {
        //        return channelContentIdList.Skip(offset).Take(limit).ToList();
        //    }

        //    var query = Q.Offset(offset).Limit(limit);
        //    await QueryWhereAsync(query, site, channel, adminId, isAllContents);
        //    QueryOrder(query, channel, string.Empty, isAllContents);

        //    if (channelContentIdList.Count == offset)
        //    {
        //        var repository = GetRepository(tableName);
        //        var pageContentList = await GetContentListAsync(repository, query);

        //        //var pageContentInfoList = await GetContentInfoListAsync(tableName, await GetCacheWhereStringAsync(site, channel, adminId, isAllContents),
        //        //    GetOrderString(channel, string.Empty, isAllContents), offset, limit);

        //        foreach (var content in pageContentList)
        //        {
        //            ListCache.Set(content);
        //            await SetEntityCacheAsync(repository, content);
        //        }

        //        var pageContentIdList = pageContentList.Select(x => (x.ChannelId, x.Id)).ToList();
        //        channelContentIdList.AddRange(pageContentIdList);
        //        return pageContentIdList;
        //    }

        //    var minList = await GetContentMinListAsync(tableName, query);
        //    return minList.Select(x => (x.ChannelId, x.Id)).ToList();

        //    //return GetCacheChannelContentIdList(tableName, await GetCacheWhereStringAsync(site, channel, adminId, isAllContents),
        //    //    GetOrderString(channel, string.Empty, isAllContents), offset, limit);
        //}

        ///// new
        //private class ContentMin
        //{
        //    public int Id { get; set; }

        //    public int ChannelId { get; set; }

        //    public string IsTop { get; set; }

        //    public DateTime? AddDate { get; set; }

        //    public DateTime? LastEditDate { get; set; }

        //    public int Taxis { get; set; }

        //    public int Hits { get; set; }

        //    public int HitsByDay { get; set; }

        //    public int HitsByWeek { get; set; }

        //    public int HitsByMonth { get; set; }
        //}

        //private async Task<IEnumerable<ContentMin>> GetContentMinListAsync(string tableName, Query query)
        //{
        //    var repository = GetRepository(tableName);
        //    var q = query.Clone();
        //    q.Select(MinColumns);
        //    return await repository.GetAllAsync<ContentMin>(q);
        //}

        private string ListCacheKey(int siteId, int channelId, int adminId, bool isAllContents) =>
            $"{nameof(ContentRepository)}:{siteId}:{channelId}:{adminId}:{isAllContents}";

        public async Task<IEnumerable<(int ChannelId, int Id)>> GetChannelContentIdListAsync(Site site, Channel channel,
            int adminId, bool isAllContents)
        {
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            var repository = GetRepository(tableName);
            var query = Q.Select(nameof(Content.ChannelId), nameof(Content.Id)).CachingGet(ListCacheKey(site.Id, channel.Id, adminId, isAllContents));

            await QueryWhereAsync(query, site, channel, adminId, isAllContents);
            QueryOrder(query, channel, string.Empty, isAllContents);

            var ccIds = await repository.GetAllAsync<(int ChannelId, int ContentId)>(query);
            return ccIds;
        }

        public async Task<int> GetCountAsync(Site site, Channel channel)
        {
            var ccIds = await GetChannelContentIdListAsync(site, channel, 0, false);
            return ccIds.Count();
        }

        public async Task<int> GetCountAsync(Site site, Channel channel, int adminId)
        {
            var ccIds = await GetChannelContentIdListAsync(site, channel, adminId, false);
            return ccIds.Count();
        }
    }
}