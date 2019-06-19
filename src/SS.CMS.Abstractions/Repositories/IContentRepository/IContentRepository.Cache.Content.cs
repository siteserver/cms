using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId);
    }
}