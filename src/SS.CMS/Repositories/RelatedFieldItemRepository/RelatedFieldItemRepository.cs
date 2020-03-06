using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class RelatedFieldItemRepository : IRelatedFieldItemRepository
    {
        private readonly Repository<RelatedFieldItem> _repository;

        public RelatedFieldItemRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<RelatedFieldItem>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId)
        {
            return Caching.GetListKey(_repository.TableName, siteId);
        }

        public async Task<int> InsertAsync(RelatedFieldItem info)
        {
            info.Taxis = await GetMaxTaxisAsync(info.SiteId, info.ParentId) + 1;

            info.Id = await _repository.InsertAsync(info, Q.CachingRemove(GetCacheKey(info.SiteId)));
            return info.Id;
        }

        public async Task<bool> UpdateAsync(RelatedFieldItem info)
        {
            return await _repository.UpdateAsync(info, Q.CachingRemove(GetCacheKey(info.SiteId)));
        }

        public async Task DeleteAsync(int siteId, int id)
        {
            var childIds = await _repository.GetAllAsync<int>(Q
                .Select(nameof(RelatedFieldItem.Id))
                .Where(nameof(RelatedFieldItem.ParentId), id)
            );
            if (childIds.Any())
            {
                foreach (var childId in childIds)
                {
                    await DeleteAsync(siteId, childId);
                }
            }
            await _repository.DeleteAsync(id, Q.CachingRemove(GetCacheKey(siteId)));
        }

        public async Task UpdateTaxisToDownAsync(int siteId, int relatedFieldId, int id, int parentId)
        {
            var selectedTaxis = await GetTaxisAsync(id);
            var item = await _repository.GetAsync(Q
                .Where(nameof(RelatedFieldItem.Taxis), ">", selectedTaxis)
                .Where(nameof(RelatedFieldItem.RelatedFieldId), relatedFieldId)
                .Where(nameof(RelatedFieldItem.ParentId), parentId)
                .OrderBy(nameof(RelatedFieldItem.Taxis)));

            if (item == null) return;

            var higherId = item.Id;
            var higherTaxis = item.Taxis;

            if (higherId != 0)
            {
                await SetTaxisAsync(siteId, id, higherTaxis);
                await SetTaxisAsync(siteId, higherId, selectedTaxis);
            }
        }

        public async Task UpdateTaxisToUpAsync(int siteId, int relatedFieldId, int id, int parentId)
        {
            var selectedTaxis = await GetTaxisAsync(id);
            var item = await _repository.GetAsync(Q
                .Where(nameof(RelatedFieldItem.Taxis), "<", selectedTaxis)
                .Where(nameof(RelatedFieldItem.RelatedFieldId), relatedFieldId)
                .Where(nameof(RelatedFieldItem.ParentId), parentId)
                .OrderByDesc(nameof(RelatedFieldItem.Taxis)));

            if (item == null) return;

            var lowerId = item.Id;
            var lowerTaxis = item.Taxis;

            if (lowerId != 0)
            {
                await SetTaxisAsync(siteId, id, lowerTaxis);
                await SetTaxisAsync(siteId, lowerId, selectedTaxis);
            }
        }

        private async Task<int> GetTaxisAsync(int id)
        {
            return await _repository.GetAsync<int>(Q
                .Select(nameof(RelatedFieldItem.Taxis))
                .Where(nameof(RelatedFieldItem.Id), id));
        }

        private async Task SetTaxisAsync(int siteId, int id, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(RelatedFieldItem.Taxis), taxis)
                .Where(nameof(RelatedFieldItem.Id), id)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        private async Task<List<RelatedFieldItem>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(RelatedFieldItem.SiteId), siteId)
                .CachingGet(GetCacheKey(siteId))
            );
        }
    }
}