using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Services;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        int GetCount(IPluginManager pluginManager, SiteInfo siteInfo, bool isChecked);

        int GetCount(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId);

        int GetCount(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked);
    }
}