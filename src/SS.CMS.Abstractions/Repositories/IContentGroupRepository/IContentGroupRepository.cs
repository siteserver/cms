using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IContentGroupRepository : IRepository
    {
        Task<int> InsertAsync(ContentGroupInfo groupInfo);

        Task<bool> UpdateAsync(ContentGroupInfo groupInfo);

        Task DeleteAsync(int siteId, string groupName);

        Task UpdateTaxisToUpAsync(int siteId, string groupName);

        Task UpdateTaxisToDownAsync(int siteId, string groupName);
    }
}