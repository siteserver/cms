using System.Threading.Tasks;
using SSCMS.Parse;

namespace SSCMS.Services
{
    public partial interface IParseManager
    {
        Task<string> ParseStlPageInContentPageAsync(string stlElement, int channelId, int contentId, int currentPageIndex, int pageCount);

        Task<string> ParseStlPageInChannelPageAsync(string stlElement, int channelId, int currentPageIndex, int pageCount, int totalNum);

        Task<string> ParseStlPageInSearchPageAsync(string stlElement, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum);

        Task<string> ParseStlPageInDynamicPageAsync(string stlElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId);

        Task<string> ParseStlPageItemsAsync(string htmlInStlPageElement, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum, bool isXmlContent, ParseType contextType);

        Task<string> ParseStlPageItemsInSearchPageAsync(string htmlInStlPageElement, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum);

        Task<string> ParseStlPageItemsInDynamicPageAsync(string htmlInStlPageElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId);

        Task<string> ParseStlPageItemInContentPageAsync(string stlElement, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum);

        Task<string> ParseStlPageItemInChannelPageAsync(string stlElement, int channelId, int currentPageIndex, int pageCount, int totalNum);

        Task<string> ParseStlPageItemInSearchPageAsync(string stlElement, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum);

        Task<string> ParseStlPageItemInDynamicPageAsync(string stlElement, int currentPageIndex,
            int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId);
    }
}
