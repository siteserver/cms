using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions.Parse;

namespace SS.CMS.Abstractions
{
    public partial interface IParseManager
    {
        Task ParseTemplateContentAsync(StringBuilder parsedBuilder, ParsePage pageInfo, ParseContext contextInfo);

        Task<string> ParseTemplatePreviewAsync(Site site, TemplateType templateType, int channelId, int contentId, string template);

        Task ParseInnerContentAsync(StringBuilder builder);

        string StlEncrypt(string stlElement);

        Task ReplacePageElementsInContentPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int channelId, int contentId, int currentPageIndex, int pageCount);

        Task ReplacePageElementsInChannelPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int channelId, int currentPageIndex, int pageCount, int totalNum);

        Task ReplacePageElementsInSearchPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum);

        Task ReplacePageElementsInDynamicPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId);
    }
}
