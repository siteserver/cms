using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IRelatedFieldItemRepository : IRepository
    {
        Task<int> InsertAsync(RelatedFieldItem info);

        Task<bool> UpdateAsync(RelatedFieldItem info);

        Task DeleteAsync(int id);

        Task<IEnumerable<RelatedFieldItem>> GetRelatedFieldItemInfoListAsync(int relatedFieldId, int parentId);

        Task UpdateTaxisToUpAsync(int id, int parentId);

        Task UpdateTaxisToDownAsync(int id, int parentId);

        Task<RelatedFieldItem> GetRelatedFieldItemInfoAsync(int id);
    }
}