using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        int Insert(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        int InsertPreview(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        int InsertWithTaxis(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo, int taxis);
    }
}
