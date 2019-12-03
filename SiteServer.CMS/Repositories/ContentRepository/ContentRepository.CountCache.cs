using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        private string GetCountCacheKey(string tableName, int siteId, int channelId, int adminId)
        {
            var repository = GetRepository(tableName);
            return adminId > 0
                ? _cache.GetCountKey(repository, siteId, channelId, adminId)
                : _cache.GetCountKey(repository, siteId, channelId);
        }

        private string GetCountCacheKey(string tableName, int siteId, int channelId)
        {
            var repository = GetRepository(tableName);
            return _cache.GetCountKey(repository, siteId, channelId);
        }

        private async Task RemoveCountCacheAsync(string tableName, int siteId, int channelId, int adminId)
        {
            var cacheKey = GetCountCacheKey(tableName, siteId, channelId, adminId);
            await _cache.RemoveAsync(cacheKey);
        }

        public async Task<int> GetCountCheckingAsync(Site site)
        {
            //var tableNames = await DataProvider.SiteRepository.GetTableNameListAsync(site);
            //var isChecked = false.ToString();

            //var count = 0;
            //foreach (var tableName in tableNames)
            //{
            //    var list = await GetCountCacheAsync(tableName);
            //    count += list.Where(x => x.SiteId == site.Id && x.IsChecked == isChecked && x.CheckedLevel != -site.CheckContentLevel && x.CheckedLevel != CheckManager.LevelInt.CaoGao)
            //        .Sum(x => x.Count);
            //}

            //return count;

            return 0;
        }

        public async Task<int> GetCountAllAsync(Site site, Channel channel)
        {
            return await GetChannelCountAllAsync(site, channel, 0);
        }

        public async Task<int> GetCountAllAsync(Site site, Channel channel, int adminId)
        {
            return await GetChannelCountAllAsync(site, channel, adminId);
        }

        public async Task<int> GetCountAsync(Site site, Channel channel)
        {
            return await GetChannelCountSelfAsync(site, channel, 0);
        }

        public async Task<int> GetCountAsync(Site site, Channel channel, int adminId)
        {
            return await GetChannelCountSelfAsync(site, channel, adminId);
        }

        //public async Task<int> GetCountAsync(Site site, Channel channel, bool isChecked)
        //{
        //    var tableName = await ChannelManager.GetTableNameAsync(site, channel);

        //    var list = await GetCountCacheAsync(tableName);
        //    return list.Where(x =>
        //            x.SiteId == site.Id && x.ChannelId == channel.Id &&
        //            x.IsChecked == isChecked.ToString())
        //        .Sum(x => x.Count);
        //}

        private async Task<int> GetChannelCountAllAsync(Site site, Channel channel, int adminId)
        {
            var count = 0;
            var channelList = new List<Channel> { channel };
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.Descendant);
            foreach (var channelId in channelIdList)
            {
                channelList.Add(await ChannelManager.GetChannelAsync(site.Id, channelId));
            }

            foreach (var info in channelList)
            {
                count += await GetChannelCountSelfAsync(site, info, adminId);
            }

            return count;
        }

        private async Task<int> GetChannelCountSelfAsync(Site site, Channel channel, int adminId)
        {
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            var cacheKey = GetCountCacheKey(tableName, site.Id, channel.Id, adminId);

            return await
                _cache.GetOrCreateIntAsync(cacheKey, async entry =>
                {
                    var repository = GetRepository(tableName);

                    var query = Q
                        .Where(nameof(Content.SiteId), site.Id)
                        .Where(nameof(Content.ChannelId), channel.Id)
                        .Where(nameof(Content.IsChecked), true.ToString())
                        .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                        .Where(nameof(Content.ChannelId), ">", 0);
                    if (adminId > 0)
                    {
                        query.Where(nameof(Content.AdminId), adminId);
                    }

                    return await repository.CountAsync(query);
                });
        }
    }
}