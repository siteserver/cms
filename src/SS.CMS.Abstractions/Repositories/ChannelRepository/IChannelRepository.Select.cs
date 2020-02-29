using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IChannelRepository
    {
        Task CacheAllAsync(Site site);

        Task<List<ChannelSummary>> GetSummariesAsync(int siteId);

        Task<Channel> GetAsync(int channelId);
    }
}