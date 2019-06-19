using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        int Insert(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        int InsertPreview(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        int InsertWithTaxis(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo, int taxis);
    }
}
