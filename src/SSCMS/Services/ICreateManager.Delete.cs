using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface ICreateManager
    {
         Task DeleteContentsAsync(Site site, int channelId, IEnumerable<int> contentIdList);

         Task DeleteContentAsync(Site site, int channelId, int contentId);

         Task DeleteChannelsAsync(Site site, IEnumerable<int> channelIdList);
    }
}
