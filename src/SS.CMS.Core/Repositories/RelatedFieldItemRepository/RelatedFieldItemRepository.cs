using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class RelatedFieldItemRepository : IRelatedFieldItemRepository
    {
        private readonly Repository<RelatedFieldItemInfo> _repository;
        public RelatedFieldItemRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<RelatedFieldItemInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(RelatedFieldItemInfo.Id);
            public const string RelatedFieldId = nameof(RelatedFieldItemInfo.RelatedFieldId);
            public const string ParentId = nameof(RelatedFieldItemInfo.ParentId);
            public const string Taxis = nameof(RelatedFieldItemInfo.Taxis);
        }

        public async Task<int> InsertAsync(RelatedFieldItemInfo info)
        {
            info.Taxis = await GetMaxTaxisAsync(info.ParentId) + 1;

            info.Id = await _repository.InsertAsync(info);
            return info.Id;
        }

        public async Task<bool> UpdateAsync(RelatedFieldItemInfo info)
        {
            return await _repository.UpdateAsync(info);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RelatedFieldItemInfo>> GetRelatedFieldItemInfoListAsync(int relatedFieldId, int parentId)
        {
            return await _repository.GetAllAsync(Q
                .Where(Attr.RelatedFieldId, relatedFieldId)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));
        }

        public async Task UpdateTaxisToUpAsync(int id, int parentId)
        {
            var selectedTaxis = await GetTaxisAsync(id);
            var result = await _repository.GetAsync<(int Id, int Taxis)?>(Q
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.Taxis, ">", selectedTaxis)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));

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
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.Taxis, "<", selectedTaxis)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

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
                .Select(Attr.Taxis)
                .Where(Attr.Id, id));
        }

        private async Task SetTaxisAsync(int id, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.Id, id)
            );
        }

        private async Task<int> GetMaxTaxisAsync(int parentId)
        {
            return await _repository.MaxAsync(Attr.Taxis, Q
                       .Where(Attr.ParentId, parentId)) ?? 0;
        }

        public async Task<RelatedFieldItemInfo> GetRelatedFieldItemInfoAsync(int id)
        {
            return await _repository.GetAsync(id);
        }
    }
}