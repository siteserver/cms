using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentGroupRepository : IRepository
    {
        private readonly Repository<ContentGroup> _repository;

        public ContentGroupRepository()
        {
            _repository = new Repository<ContentGroup>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), new Redis(WebConfigUtils.RedisConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(ContentGroup group)
        {
            group.Taxis = await GetMaxTaxisAsync(group.SiteId) + 1;

            await _repository.InsertAsync(group, Q
                .CachingRemove(GetCacheKey(group.SiteId))
            );
        }

        public async Task UpdateAsync(ContentGroup group)
        {
            await _repository.UpdateAsync(group, Q
                .CachingRemove(GetCacheKey(group.SiteId))
            );
        }

        public async Task DeleteAsync(int siteId, string groupName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.GroupName), groupName)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task DeleteAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ContentGroup.SiteId), siteId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task UpdateTaxisToUpAsync(int siteId, string groupName)
        {
            var taxis = await GetTaxisAsync(siteId, groupName);
            var result = await _repository.GetAsync<(string GroupName, int Taxis)?>(Q
                .Select(nameof(ContentGroup.GroupName), nameof(ContentGroup.Taxis))
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.Taxis), ">", taxis)
                .OrderBy(nameof(ContentGroup.Taxis)));

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

        public async Task UpdateTaxisToDownAsync(int siteId, string groupName)
        {
            var taxis = await GetTaxisAsync(siteId, groupName);
            var result = await _repository.GetAsync<(string GroupName, int Taxis)?>(Q
                .Select(nameof(ContentGroup.GroupName), nameof(ContentGroup.Taxis))
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.Taxis), "<", taxis)
                .OrderByDesc(nameof(ContentGroup.Taxis)));

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

        private async Task<int> GetTaxisAsync(int siteId, string groupName)
        {
            return await _repository.GetAsync<int>(Q
                .Select(nameof(ContentGroup.Taxis))
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.GroupName), groupName)
            );
        }

        private async Task SetTaxisAsync(int siteId, string groupName, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(ContentGroup.Taxis), taxis)
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.GroupName), groupName)
                .CachingRemove(GetCacheKey(siteId))
            );
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
            var max = await _repository.MaxAsync(nameof(ContentGroup.Taxis), Q
                .Where(nameof(ContentGroup.SiteId), siteId)
            );
            return max ?? 0;
        }

        public async Task<bool> IsExistsAsync(int siteId, string groupName)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.GroupName), groupName)
            );
        }

        public async Task<ContentGroup> GetContentGroupAsync(int siteId, string groupName)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.GroupName), groupName)
            );
        }

        public async Task<ContentGroup> GetAsync(int siteId, int groupId)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.Id), groupId)
            );
        }

        public async Task<List<ContentGroup>> GetContentGroupsAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(ContentGroup.SiteId), siteId)
                .OrderBy(nameof(ContentGroup.Taxis))
                .OrderBy(nameof(ContentGroup.GroupName))
            );
        }
    }
}