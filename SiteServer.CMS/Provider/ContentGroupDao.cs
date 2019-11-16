using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Provider
{
    public class ContentGroupDao : IRepository
    {
        private readonly Repository<ContentGroup> _repository;

        public ContentGroupDao()
        {
            _repository = new Repository<ContentGroup>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(ContentGroup group)
        {
            group.Taxis = await GetMaxTaxisAsync(group.SiteId) + 1;

            await _repository.InsertAsync(group);
            ContentGroupManager.ClearCache();
        }

        public async Task UpdateAsync(ContentGroup group)
        {
            await _repository.UpdateAsync(group);
            ContentGroupManager.ClearCache();
        }

        public async Task DeleteAsync(int siteId, string groupName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ContentGroup.SiteId), siteId)
                .Where(nameof(ContentGroup.GroupName), groupName)
            );

            ContentGroupManager.ClearCache();
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

            ContentGroupManager.ClearCache();
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

            ContentGroupManager.ClearCache();
        }

        public async Task<Dictionary<int, List<ContentGroup>>> GetAllContentGroupsAsync()
        {
            var allDict = new Dictionary<int, List<ContentGroup>>();

            var groupList = await _repository.GetAllAsync(Q
                .OrderByDesc(nameof(ContentGroup.Taxis))
                .OrderBy(nameof(ContentGroup.GroupName)));

            foreach (var group in groupList)
            {
                allDict.TryGetValue(group.SiteId, out var list);

                if (list == null)
                {
                    list = new List<ContentGroup>();
                }

                list.Add(group);

                allDict[group.SiteId] = list;
            }

            return allDict;
        }

        private async Task<int> GetTaxisAsync(int siteId, string groupName)
        {
            return await _repository.GetAsync<int>(Q
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
            );
        }

        private async Task<int> GetMaxTaxisAsync(int siteId)
        {
            var max = await _repository.MaxAsync(nameof(ContentGroup.Taxis), Q
                .Where(nameof(ContentGroup.SiteId), siteId)
            );
            return max ?? 0;
        }
    }
}