using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        List<InputListItem> GetContentsColumns(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll);

        List<ContentColumn> GetContentColumns(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll);
    }
}
