using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId);
    }
}