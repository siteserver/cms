using System.Threading.Tasks;

namespace SSCMS.Repositories
{
    public partial interface IChannelRepository
    {
        Task UpdateTaxisAsync(int siteId, int parentId, int channelId, bool isUp);

        Task DropAsync(int siteId, int sourceId, int targetId, string dropType);
    }
}
