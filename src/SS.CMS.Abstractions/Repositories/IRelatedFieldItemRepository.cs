using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IRelatedFieldItemRepository : IRepository
    {
        int Insert(RelatedFieldItemInfo info);

        bool Update(RelatedFieldItemInfo info);

        Task DeleteAsync(int id);

        IList<RelatedFieldItemInfo> GetRelatedFieldItemInfoList(int relatedFieldId, int parentId);

        void UpdateTaxisToUp(int id, int parentId);

        void UpdateTaxisToDown(int id, int parentId);

        RelatedFieldItemInfo GetRelatedFieldItemInfo(int id);
    }
}