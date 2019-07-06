using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IChannelGroupRepository
    {
        Task<Dictionary<int, List<ChannelGroupInfo>>> GetAllChannelGroupsAsync();

        Task<bool> IsExistsAsync(int siteId, string groupName);

        Task<ChannelGroupInfo> GetChannelGroupInfoAsync(int siteId, string groupName);

        Task<List<string>> GetGroupNameListAsync(int siteId);

        Task<List<ChannelGroupInfo>> GetChannelGroupInfoListAsync(int siteId);
    }
}
