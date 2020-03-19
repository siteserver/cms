using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IChannelRepository
    {
        Task<Channel> ImportGetAsync(int channelId);

        Task<List<string>> ImportGetIndexNameListAsync(int siteId);

        Task<int> ImportGetCountAsync(int siteId, int parentId);

        Task<int> ImportGetIdAsync(int siteId, string orderString);

        Task<int> ImportGetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive);

        Task<string> ImportGetOrderStringInSiteAsync(int siteId, int channelId);
    }
}
