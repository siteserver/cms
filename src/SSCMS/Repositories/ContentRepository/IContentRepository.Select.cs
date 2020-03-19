using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IContentRepository
    {
        Task CacheAllListAndCountAsync(Site site, List<ChannelSummary> channelSummaries);

        Task CacheAllEntityAsync(Site site, List<ChannelSummary> channelSummaries);

        Task<int> GetCountAsync(Site site, IChannelSummary channel);

        Task<Content> GetAsync(Site site, int channelId, int contentId);

        Task<Content> GetAsync(Site site, Channel channel, int contentId);

        Task<List<int>> GetContentIdsAsync(Site site, Channel channel);

        Task<List<int>> GetContentIdsCheckedAsync(Site site, Channel channel);

        Task<List<int>> GetContentIdsAsync(Site site, Channel channel, bool isPeriods, string dateFrom, string dateTo, bool? checkedState);

        Task<List<int>> GetChannelIdsCheckedByLastModifiedDateHourAsync(Site site, int hour);

        Task<List<ContentSummary>> GetSummariesAsync(Site site, Channel channel, bool isAllContents);
    }
}
