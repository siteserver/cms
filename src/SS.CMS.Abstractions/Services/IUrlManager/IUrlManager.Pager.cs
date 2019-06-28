using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUrlManager
    {
        Task<string> GetPagerUrlInChannelPageAsync(string type, SiteInfo siteInfo, ChannelInfo nodeInfo, int index, int currentPageIndex, int pageCount, bool isLocal);

        Task<string> GetPagerUrlInContentPageAsync(string type, SiteInfo siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isLocal);

        string GetPagerClickStringInSearchPage(string type, string ajaxDivId, int index, int currentPageIndex, int pageCount);

        Task<string> GetPagerJsMethodInDynamicPageAsync(string type, SiteInfo siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivId, bool isLocal);
    }
}
