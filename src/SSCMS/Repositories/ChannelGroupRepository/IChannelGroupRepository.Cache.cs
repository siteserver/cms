using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IChannelGroupRepository
    {
        Task<List<string>> GetGroupNamesAsync(int siteId);

        Task<bool> IsExistsAsync(int siteId, string groupName);
    }
}