using System.Collections.Generic;
using System.Linq;
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

        public int Insert(RelatedFieldItemInfo info)
        {
            info.Taxis = GetMaxTaxis(info.ParentId) + 1;

            info.Id = _repository.Insert(info);
            return info.Id;
        }

        public bool Update(RelatedFieldItemInfo info)
        {
            return _repository.Update(info);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public IList<RelatedFieldItemInfo> GetRelatedFieldItemInfoList(int relatedFieldId, int parentId)
        {
            return _repository.GetAll(Q
                .Where(Attr.RelatedFieldId, relatedFieldId)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis)).ToList();
        }

        public void UpdateTaxisToUp(int id, int parentId)
        {
            var selectedTaxis = GetTaxis(id);
            var result = _repository.Get<(int Id, int Taxis)?>(Q
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.Taxis, ">", selectedTaxis)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));

            if (result == null) return;

            var higherId = result.Value.Id;
            var higherTaxis = result.Value.Taxis;

            if (higherId != 0)
            {
                SetTaxis(id, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
            }
        }

        public void UpdateTaxisToDown(int id, int parentId)
        {
            var selectedTaxis = GetTaxis(id);
            var result = _repository.Get<(int Id, int Taxis)?>(Q
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.Taxis, "<", selectedTaxis)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (result == null) return;

            var lowerId = result.Value.Id;
            var lowerTaxis = result.Value.Taxis;

            if (lowerId != 0)
            {
                SetTaxis(id, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
            }
        }

        private int GetTaxis(int id)
        {
            return _repository.Get<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.Id, id));
        }

        private void SetTaxis(int id, int taxis)
        {
            _repository.Update(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.Id, id)
            );
        }

        private int GetMaxTaxis(int parentId)
        {
            return _repository.Max(Attr.Taxis, Q
                       .Where(Attr.ParentId, parentId)) ?? 0;
        }

        public RelatedFieldItemInfo GetRelatedFieldItemInfo(int id)
        {
            return _repository.Get(id);
        }
    }
}