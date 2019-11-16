using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
	public class ChannelGroupDao : IRepository
    {
        private readonly Repository<ChannelGroup> _repository;

        public ChannelGroupDao()
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
            ChannelGroupManager.ClearCache();
        }

		public async Task UpdateAsync(ChannelGroup group) 
		{
            await _repository.UpdateAsync(group);
            ChannelGroupManager.ClearCache();
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
                await DataProvider.ChannelDao.UpdateAsync(channelInfo);
		    }

		    ChannelGroupManager.ClearCache();
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

            ChannelGroupManager.ClearCache();
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

            ChannelGroupManager.ClearCache();
        }

	    public async Task<Dictionary<int, List<ChannelGroup>>> GetAllChannelGroupsAsync()
	    {
            var allDict = new Dictionary<int, List<ChannelGroup>>();

            var groupList = await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(ChannelGroup.Taxis))
                .OrderBy(nameof(ChannelGroup.GroupName)));

            foreach (var group in groupList)
            {
                allDict.TryGetValue(group.SiteId, out var list);

                if (list == null)
                {
                    list = new List<ChannelGroup>();
                }

                list.Add(group);

                allDict[group.SiteId] = list;
            }

            return allDict;
        }
    }
}
