using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsPageContents
    {
        public const string Route = "sys/stl/actions/pagecontents";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int siteId, int pageChannelId, int templateId, int totalNum, int pageCount,
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
    stlPageContentsElement: '{TranslateUtils.EncryptStringBySecretKey(stlPageContentsElement)}'
}}";
        }
    }
}