using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface IChannelGroupRepository : IRepository
    {
        Task<int> InsertAsync(ChannelGroup groupInfo);

        Task<bool> UpdateAsync(ChannelGroup groupInfo);

        Task DeleteAsync(int siteId, string groupName);

        Task UpdateTaxisToUpAsync(int siteId, string groupName);

        Task UpdateTaxisToDownAsync(int siteId, string groupName);
    }
}
