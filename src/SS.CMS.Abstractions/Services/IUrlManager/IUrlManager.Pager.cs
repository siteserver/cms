using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IUrlManager
    {
        string GetPagerUrlInChannelPage(string type, SiteInfo siteInfo, ChannelInfo nodeInfo, int index, int currentPageIndex, int pageCount, bool isLocal);

        string GetPagerUrlInContentPage(string type, SiteInfo siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isLocal);

        string GetPagerClickStringInSearchPage(string type, string ajaxDivId, int index, int currentPageIndex, int pageCount);

        string GetPagerJsMethodInDynamicPage(string type, SiteInfo siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivId, bool isLocal);
    }
}
