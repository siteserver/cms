using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ISpecialRepository : IRepository
    {
        Task<int> InsertAsync(SpecialInfo specialInfo);

        Task<bool> UpdateAsync(SpecialInfo specialInfo);

        Task<SpecialInfo> DeleteAsync(int siteId, int specialId);

        Task<bool> IsTitleExistsAsync(int siteId, string title);

        Task<bool> IsUrlExistsAsync(int siteId, string url);

        Task<IEnumerable<SpecialInfo>> GetSpecialInfoListAsync(int siteId);

        Task<IEnumerable<SpecialInfo>> GetSpecialInfoListAsync(int siteId, string keyword);

        Task<Dictionary<int, SpecialInfo>> GetSpecialInfoDictionaryBySiteIdAsync(int siteId);
    }
}