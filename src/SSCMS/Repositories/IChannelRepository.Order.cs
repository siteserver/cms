using System.Threading.Tasks;

namespace SSCMS.Repositories
{
    public partial interface IChannelRepository
    {
        Task UpdateTaxisAsync(int siteId, int parentId, int channelId, bool isUp);
    }
}
