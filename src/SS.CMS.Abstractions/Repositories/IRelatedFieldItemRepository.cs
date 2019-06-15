using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IRelatedFieldItemRepository : IRepository
    {
        int Insert(RelatedFieldItemInfo info);

        bool Update(RelatedFieldItemInfo info);

        void Delete(int id);

        IList<RelatedFieldItemInfo> GetRelatedFieldItemInfoList(int relatedFieldId, int parentId);

        void UpdateTaxisToUp(int id, int parentId);

        void UpdateTaxisToDown(int id, int parentId);

        RelatedFieldItemInfo GetRelatedFieldItemInfo(int id);
    }
}