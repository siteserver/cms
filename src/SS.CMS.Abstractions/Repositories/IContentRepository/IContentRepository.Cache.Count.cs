using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        int GetCount(SiteInfo siteInfo, bool isChecked);

        int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId);

        int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked);
    }
}