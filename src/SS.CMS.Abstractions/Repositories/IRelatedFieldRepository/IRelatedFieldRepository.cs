using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IRelatedFieldRepository : IRepository
    {
        Task<int> InsertAsync(RelatedFieldInfo relatedFieldInfo);

        Task<bool> UpdateAsync(RelatedFieldInfo relatedFieldInfo);

        Task DeleteAsync(int id);

        Task<RelatedFieldInfo> GetRelatedFieldInfoAsync(int id);

        Task<RelatedFieldInfo> GetRelatedFieldInfoAsync(int siteId, string title);

        Task<string> GetTitleAsync(int id);

        Task<IEnumerable<RelatedFieldInfo>> GetRelatedFieldInfoListAsync(int siteId);

        Task<IEnumerable<string>> GetTitleListAsync(int siteId);

        Task<string> GetImportTitleAsync(int siteId, string relatedFieldName);
    }
}