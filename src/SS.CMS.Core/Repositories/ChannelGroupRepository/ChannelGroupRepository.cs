using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelGroupRepository : IChannelGroupRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly Repository<ChannelGroupInfo> _repository;

        public ChannelGroupRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(ChannelGroupRepository));
            _repository = new Repository<ChannelGroupInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string GroupName = nameof(ChannelGroupInfo.GroupName);
            public const string SiteId = nameof(ChannelGroupInfo.SiteId);
            public const string Taxis = nameof(ChannelGroupInfo.Taxis);
        }

        public async Task<int> InsertAsync(ChannelGroupInfo groupInfo)
        {
            var maxTaxis = await GetMaxTaxisAsync(groupInfo.SiteId);
            groupInfo.Taxis = maxTaxis + 1;

            groupInfo.Id = await _repository.InsertAsync(groupInfo);
            await _cache.RemoveAsync(_cacheKey);
            return groupInfo.Id;
        }

        public async Task<bool> UpdateAsync(ChannelGroupInfo groupInfo)
        {
            var updated = await _repository.UpdateAsync(groupInfo);

            if (updated)
            {
                await _cache.RemoveAsync(_cacheKey);
            }

            return updated;
        }

        public async Task DeleteAsync(int siteId, string groupName)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName));

            // var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, siteId), ScopeType.All, groupName, string.Empty, string.Empty);
            // foreach (var channelId in channelIdList)
            // {
            //     var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            //     var groupNameList = TranslateUtils.StringCollectionToStringList(channelInfo.GroupNameCollection);
            //     groupNameList.Remove(groupName);
            //     channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(groupNameList);
            //     DataProvider.ChannelRepository.Update(channelInfo);
            // }

            await _cache.RemoveAsync(_cacheKey);
        }

        private async Task<int> GetTaxisAsync(int siteId, string groupName)
        {
            return await _repository.GetAsync<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.GroupName, groupName));
        }

        private async Task SetTaxisAsync(int siteId, string groupName, int taxis)
        {
            await _repository.UpdateAsync(Q
                            .Set(Attr.Taxis, taxis)
                            .Where(Attr.SiteId, siteId)
                            .Where(Attr.GroupName, groupName)
                        );
        }

        private async Task<int> GetMaxTaxisAsync(int siteId)
        {
            return await _repository.MaxAsync(Attr.Taxis, Q
                       .Where(Attr.SiteId, siteId)
                   ) ?? 0;
        }

        public async Task UpdateTaxisToUpAsync(int siteId, string groupName)
        {
            var taxis = await GetTaxisAsync(siteId, groupName);
            var result = await _repository.GetAsync<(string GroupName, int Taxis)?>(Q
                .Select(Attr.GroupName, Attr.Taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Taxis, ">", taxis)
                .OrderBy(Attr.Taxis));

            var higherGroupName = string.Empty;
            var higherTaxis = 0;
            if (result != null)
            {
                higherGroupName = result.Value.GroupName;
                higherTaxis = result.Value.Taxis;
            }

            if (!string.IsNullOrEmpty(higherGroupName))
            {
                await SetTaxisAsync(siteId, groupName, higherTaxis);
                await SetTaxisAsync(siteId, higherGroupName, taxis);
            }

            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task UpdateTaxisToDownAsync(int siteId, string groupName)
        {
            var taxis = await GetTaxisAsync(siteId, groupName);
            var result = await _repository.GetAsync<(string GroupName, int Taxis)?>(Q
                .Select(Attr.GroupName, Attr.Taxis)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Taxis, "<", taxis)
                .OrderByDesc(Attr.Taxis));

            var lowerGroupName = string.Empty;
            var lowerTaxis = 0;
            if (result != null)
            {
                lowerGroupName = result.Value.GroupName;
                lowerTaxis = result.Value.Taxis;
            }

            if (!string.IsNullOrEmpty(lowerGroupName))
            {
                await SetTaxisAsync(siteId, groupName, lowerTaxis);
                await SetTaxisAsync(siteId, lowerGroupName, taxis);
            }

            await _cache.RemoveAsync(_cacheKey);
        }

        private async Task<Dictionary<int, List<ChannelGroupInfo>>> GetAllChannelGroupsToCacheAsync()
        {
            var allDict = new Dictionary<int, List<ChannelGroupInfo>>();

            var groupList = await _repository.GetAllAsync(Q
                .OrderByDesc(Attr.Taxis)
                .OrderBy(Attr.GroupName));

            foreach (var group in groupList)
            {
                allDict.TryGetValue(group.SiteId, out var list);

                if (list == null)
                {
                    list = new List<ChannelGroupInfo>();
                }

                list.Add(group);

                allDict[group.SiteId] = list;
            }

            return allDict;
        }
    }
}
