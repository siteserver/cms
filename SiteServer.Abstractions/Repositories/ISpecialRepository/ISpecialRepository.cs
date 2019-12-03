using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface ISpecialRepository : IRepository
    {
        Task<int> InsertAsync(Special specialInfo);

        Task<bool> UpdateAsync(Special specialInfo);

        Task<Special> DeleteAsync(int siteId, int specialId);

        Task<bool> IsTitleExistsAsync(int siteId, string title);

        Task<bool> IsUrlExistsAsync(int siteId, string url);

        Task<IEnumerable<Special>> GetSpecialInfoListAsync(int siteId);

        Task<IEnumerable<Special>> GetSpecialInfoListAsync(int siteId, string keyword);

        Task<Dictionary<int, Special>> GetSpecialInfoDictionaryBySiteIdAsync(int siteId);
    }
}