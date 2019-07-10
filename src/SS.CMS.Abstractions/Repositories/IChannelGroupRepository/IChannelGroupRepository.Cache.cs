using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IChannelGroupRepository
    {
        Task<Dictionary<int, List<ChannelGroup>>> GetAllChannelGroupsAsync();

        Task<bool> IsExistsAsync(int siteId, string groupName);

        Task<ChannelGroup> GetChannelGroupInfoAsync(int siteId, string groupName);

        Task<List<string>> GetGroupNameListAsync(int siteId);

        Task<List<ChannelGroup>> GetChannelGroupInfoListAsync(int siteId);
    }
}
