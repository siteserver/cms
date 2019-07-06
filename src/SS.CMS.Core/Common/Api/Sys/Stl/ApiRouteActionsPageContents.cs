using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Api.Sys.Stl
{
    public class ApiRouteActionsPageContents
    {
        public const string Route = "sys/stl/actions/pagecontents";

        public static string GetUrl()
        {
            return PageUtils.Combine(Constants.ApiPrefix, Route);
        }

        public static string GetParameters(ISettingsManager settingsManager, int siteId, int pageChannelId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement)
        {
            return $@"
{{
    siteId: {siteId},
    pageChannelId: {pageChannelId},
    templateId: {templateId},
    totalNum: {totalNum},
    pageCount: {pageCount},
    currentPageIndex: {currentPageIndex},
    stlPageContentsElement: '{settingsManager.Encrypt(stlPageContentsElement)}'
}}";
        }
    }
}