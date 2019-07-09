using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<IEnumerable<int>> GetContentIdListAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyUserId, int offset, int limit);
    }
}