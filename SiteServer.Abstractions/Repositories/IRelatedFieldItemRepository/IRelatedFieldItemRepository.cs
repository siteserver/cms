using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
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