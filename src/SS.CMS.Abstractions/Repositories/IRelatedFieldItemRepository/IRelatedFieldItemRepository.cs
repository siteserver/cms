using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IRelatedFieldItemRepository : IRepository
    {
        Task<int> InsertAsync(RelatedFieldItemInfo info);

        Task<bool> UpdateAsync(RelatedFieldItemInfo info);

        Task DeleteAsync(int id);

        Task<IEnumerable<RelatedFieldItemInfo>> GetRelatedFieldItemInfoListAsync(int relatedFieldId, int parentId);

        Task UpdateTaxisToUpAsync(int id, int parentId);

        Task UpdateTaxisToDownAsync(int id, int parentId);

        Task<RelatedFieldItemInfo> GetRelatedFieldItemInfoAsync(int id);
    }
}