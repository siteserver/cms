using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public class RelatedFieldItemRepository : IRepository
    {
        private readonly Repository<RelatedFieldItem> _repository;

        public RelatedFieldItemRepository()
        {
            _repository = new Repository<RelatedFieldItem>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(RelatedFieldItem info)
        {
            info.Taxis = await GetMaxTaxisAsync(info.ParentId) + 1;

            info.Id = await _repository.InsertAsync(info);
            return info.Id;
        }

        public async Task<bool> UpdateAsync(RelatedFieldItem info)
        {
            return await _repository.UpdateAsync(info);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RelatedFieldItem>> GetRelatedFieldItemInfoListAsync(int relatedFieldId, int parentId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(RelatedFieldItem.RelatedFieldId), relatedFieldId)
                .Where(nameof(RelatedFieldItem.ParentId), parentId)
                .OrderBy(nameof(RelatedFieldItem.Taxis)));
        }

        public async Task UpdateTaxisToUpAsync(int id, int parentId)
        {
            var selectedTaxis = await GetTaxisAsync(id);
            var result = await _repository.GetAsync<(int Id, int Taxis)?>(Q
                .Select(nameof(RelatedFieldItem.Id), nameof(RelatedFieldItem.Taxis))
                .Where(nameof(RelatedFieldItem.Taxis), ">", selectedTaxis)
                .Where(nameof(RelatedFieldItem.ParentId), parentId)
                .OrderBy(nameof(RelatedFieldItem.Taxis)));

            if (result == null) return;

            var higherId = result.Value.Id;
            var higherTaxis = result.Value.Taxis;

            if (higherId != 0)
            {
                await SetTaxisAsync(id, higherTaxis);
                await SetTaxisAsync(higherId, selectedTaxis);
            }
        }

        public async Task UpdateTaxisToDownAsync(int id, int parentId)
        {
            var selectedTaxis = await GetTaxisAsync(id);
            var result = await _repository.GetAsync<(int Id, int Taxis)?>(Q
                .Select(nameof(RelatedFieldItem.Id), nameof(RelatedFieldItem.Taxis))
                .Where(nameof(RelatedFieldItem.Taxis), "<", selectedTaxis)
                .Where(nameof(RelatedFieldItem.ParentId), parentId)
                .OrderByDesc(nameof(RelatedFieldItem.Taxis)));

            if (result == null) return;

            var lowerId = result.Value.Id;
            var lowerTaxis = result.Value.Taxis;

            if (lowerId != 0)
            {
                await SetTaxisAsync(id, lowerTaxis);
                await SetTaxisAsync(lowerId, selectedTaxis);
            }
        }

        private async Task<int> GetTaxisAsync(int id)
        {
            return await _repository.GetAsync<int>(Q
                .Select(nameof(RelatedFieldItem.Taxis))
                .Where(nameof(RelatedFieldItem.Id), id));
        }

        private async Task SetTaxisAsync(int id, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(RelatedFieldItem.Taxis), taxis)
                .Where(nameof(RelatedFieldItem.Id), id)
            );
        }

        private async Task<int> GetMaxTaxisAsync(int parentId)
        {
            return await _repository.MaxAsync(nameof(RelatedFieldItem.Taxis), Q
                       .Where(nameof(RelatedFieldItem.ParentId), parentId)) ?? 0;
        }

        public async Task<RelatedFieldItem> GetAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
    }
}