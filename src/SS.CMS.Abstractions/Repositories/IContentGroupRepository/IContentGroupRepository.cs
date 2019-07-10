using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IContentGroupRepository : IRepository
    {
        Task<int> InsertAsync(ContentGroup groupInfo);

        Task<bool> UpdateAsync(ContentGroup groupInfo);

        Task DeleteAsync(int siteId, string groupName);

        Task UpdateTaxisToUpAsync(int siteId, string groupName);

        Task UpdateTaxisToDownAsync(int siteId, string groupName);
    }
}