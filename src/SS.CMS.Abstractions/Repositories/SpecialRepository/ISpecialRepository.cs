using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public partial interface ISpecialRepository : IRepository
    {
        Task<int> InsertAsync(Special special);

        Task UpdateAsync(Special special);

        Task<bool> IsTitleExistsAsync(int siteId, string title);

        Task<bool> IsUrlExistsAsync(int siteId, string url);

        Task<List<Special>> GetSpecialListAsync(int siteId);
    }
}