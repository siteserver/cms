using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IChannelGroupRepository : IRepository
    {
        Task<int> InsertAsync(ChannelGroupInfo groupInfo);

        Task<bool> UpdateAsync(ChannelGroupInfo groupInfo);

        Task DeleteAsync(int siteId, string groupName);

        Task UpdateTaxisToUpAsync(int siteId, string groupName);

        Task UpdateTaxisToDownAsync(int siteId, string groupName);
    }
}
