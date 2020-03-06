using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IChannelGroupRepository
    {
        Task<List<string>> GetGroupNamesAsync(int siteId);

        Task<bool> IsExistsAsync(int siteId, string groupName);
    }
}