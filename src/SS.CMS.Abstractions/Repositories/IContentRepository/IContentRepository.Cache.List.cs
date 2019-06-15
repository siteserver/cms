using System.Collections.Generic;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        List<int> GetContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId, int offset, int limit);
    }
}