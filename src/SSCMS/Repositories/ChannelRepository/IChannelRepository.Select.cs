using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IChannelRepository
    {
        Task CacheAllAsync(Site site);

        Task<List<ChannelSummary>> GetSummariesAsync(int siteId);

        Task<Channel> GetAsync(int channelId);
    }
}