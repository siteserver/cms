using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
	public class ChannelGroupRepository : IRepository
    {
        private readonly Repository<ChannelGroup> _repository;

        public ChannelGroupRepository()
        {
            _repository = new Repository<ChannelGroup>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(ChannelGroup group) 
		{
            group.Taxis = await GetMaxTaxisAsync(group.SiteId) + 1;

            await _repository.InsertAsync(group);
        }

		public async Task UpdateAsync(ChannelGroup group) 
		{
            await _repository.UpdateAsync(group);
        }

        public async Task DeleteAsync(int siteId, string groupName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.GroupName), groupName)
            );

		    var channelIdList = await ChannelManager.GetChannelIdListAsync(await ChannelManager.GetChannelAsync(siteId, siteId), EScopeType.All, groupName, string.Empty, string.Empty);
		    foreach (var channelId in channelIdList)
		    {
		        var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                channelInfo.GroupNames.Remove(groupName);
                await DataProvider.ChannelRepository.UpdateAsync(channelInfo);
		    }
        }

        private async Task<int> GetTaxisAsync(int siteId, string groupName)
        {
            return await _repository.GetAsync<int>(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.GroupName), groupName)
            );
        }

        private async Task SetTaxisAsync(int siteId, string groupName, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(ChannelGroup.Taxis), taxis)
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.GroupName), groupName)
            );
        }

        private async Task<int> GetMaxTaxisAsync(int siteId)
        {
            var max = await _repository.MaxAsync(nameof(ChannelGroup.Taxis), Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
            );
            return max ?? 0;
        }

        public async Task UpdateTaxisToUpAsync(int siteId, string groupName)
        {
            var taxis = await GetTaxisAsync(siteId, groupName);
            var result = await _repository.GetAsync<(string GroupName, int Taxis)?>(Q
                .Select(nameof(ChannelGroup.GroupName), nameof(ChannelGroup.Taxis))
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.Taxis), ">", taxis)
                .OrderBy(nameof(ChannelGroup.Taxis)));

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
        }

        public async Task UpdateTaxisToDownAsync(int siteId, string groupName)
        {
            var taxis = await GetTaxisAsync(siteId, groupName);
            var result = await _repository.GetAsync<(string GroupName, int Taxis)?>(Q
                .Select(nameof(ChannelGroup.GroupName), nameof(ChannelGroup.Taxis))
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.Taxis), "<", taxis)
                .OrderByDesc(nameof(ChannelGroup.Taxis)));

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
        }

        public async Task<bool> IsExistsAsync(int siteId, string groupName)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.GroupName), groupName)
            );
        }

        public async Task<ChannelGroup> GetAsync(int siteId, string groupName)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .Where(nameof(ChannelGroup.GroupName), groupName)
            );
        }

        public async Task<IEnumerable<string>> GetGroupNameListAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(ChannelGroup.GroupName))
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .OrderByDesc(nameof(ChannelGroup.Taxis))
                .OrderBy(nameof(ChannelGroup.GroupName))

            );
        }

        public async Task<IEnumerable<ChannelGroup>> GetChannelGroupListAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(ChannelGroup.SiteId), siteId)
                .OrderByDesc(nameof(ChannelGroup.Taxis))
                .OrderBy(nameof(ChannelGroup.GroupName))
            );
        }
    }
}
