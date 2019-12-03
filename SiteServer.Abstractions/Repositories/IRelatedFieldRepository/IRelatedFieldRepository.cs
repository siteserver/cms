using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface IRelatedFieldRepository : IRepository
    {
        Task<int> InsertAsync(RelatedField relatedFieldInfo);

        Task<bool> UpdateAsync(RelatedField relatedFieldInfo);

        Task DeleteAsync(int id);

        Task<RelatedField> GetRelatedFieldInfoAsync(int id);

        Task<RelatedField> GetRelatedFieldInfoAsync(int siteId, string title);

        Task<string> GetTitleAsync(int id);

        Task<IEnumerable<RelatedField>> GetRelatedFieldInfoListAsync(int siteId);

        Task<IEnumerable<string>> GetTitleListAsync(int siteId);

        Task<string> GetImportTitleAsync(int siteId, string relatedFieldName);
    }
}