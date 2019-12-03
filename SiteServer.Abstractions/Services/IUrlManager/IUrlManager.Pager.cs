using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IUrlManager
    {
        Task<string> GetPagerUrlInChannelPageAsync(string type, Site siteInfo, Channel nodeInfo, int index, int currentPageIndex, int pageCount, bool isLocal);

        Task<string> GetPagerUrlInContentPageAsync(string type, Site siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isLocal);

        string GetPagerClickStringInSearchPage(string type, string ajaxDivId, int index, int currentPageIndex, int pageCount);

        Task<string> GetPagerJsMethodInDynamicPageAsync(string type, Site siteInfo, int channelId, int contentId, int index, int currentPageIndex, int pageCount, bool isPageRefresh, string ajaxDivId, bool isLocal);
    }
}
