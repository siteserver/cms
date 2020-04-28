using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class ChannelGroupRepository : IChannelGroupRepository
    {
        private readonly Repository<ChannelGroup> _repository;

        public ChannelGroupRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ChannelGroup>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(ChannelGroup group)
        {
            group.Taxis = await GetMaxTaxisAsync(group.SiteId) + 1;

            await _repository.InsertAsync(group, Q
                .CachingRemove(GetCacheKey(group.SiteId))
            );
        }

        public async Task UpdateAsync(ChannelGroup group)
        {
            await _repository.UpdateAsync(group, Q
                .CachingRemove(GetCacheKey(group.SiteId))
            );
        }

        public async Task DeleteAsync(int siteId, string groupName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.GroupName), groupName)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task UpdateTaxisDownAsync(int siteId, int groupId, int taxis)
        {
            var higherGroup = await _repository.GetAsync<ChannelGroup>(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.Taxis), ">", taxis)
                .WhereNot(nameof(ChannelGroup.Id), groupId)
                .OrderBy(nameof(ChannelGroup.Taxis)));

            if (higherGroup != null)
            {
                await SetTaxisAsync(groupId, higherGroup.Taxis);
                await SetTaxisAsync(higherGroup.Id, taxis);
            }
        }

        public async Task UpdateTaxisUpAsync(int siteId, int groupId, int taxis)
        {
            var lowerGroup = await _repository.GetAsync<ChannelGroup>(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.Taxis), "<", taxis)
                .WhereNot(nameof(ChannelGroup.Id), groupId)
                .OrderByDesc(nameof(ChannelGroup.Taxis)));

            if (lowerGroup != null)
            {
                await SetTaxisAsync(groupId, lowerGroup.Taxis);
                await SetTaxisAsync(lowerGroup.Id, taxis);
            }
        }

        private async Task SetTaxisAsync(int groupId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(ChannelGroup.Taxis), taxis)
                .Where(nameof(ChannelGroup.Id), groupId)
            );
        }

        private async Task<int> GetMaxTaxisAsync(int siteId)
        {
            var max = await _repository.MaxAsync(nameof(ChannelGroup.Taxis), Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
            );
            return max ?? 0;
        }

        public async Task<ChannelGroup> GetAsync(int siteId, int groupId)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.Id), groupId)
            );
        }

        public async Task<List<ChannelGroup>> GetChannelGroupsAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .OrderBy(nameof(ChannelGroup.Taxis))
                .OrderBy(nameof(ChannelGroup.GroupName))
            );
        }
    }
}