using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        List<int> GetContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId, int offset, int limit);
    }
}