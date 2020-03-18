using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Abstractions.Dto;

namespace SSCMS.Abstractions
{
    public partial interface IRelatedFieldItemRepository
    {
        Task<List<RelatedFieldItem>> GetListAsync(int siteId, int relatedFieldId, int parentId);

        Task<RelatedFieldItem> GetAsync(int siteId, int id);

        Task<List<Cascade<int>>> GetCascadesAsync(int siteId, int relatedFieldId, int parentId);
    }
}