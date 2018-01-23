using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsPageContents
    {
        public const string Route = "sys/stl/actions/pagecontents";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int siteId, int pageNodeId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement)
        {
            return $@"
{{
    siteId: {siteId},
    pageNodeId: {pageNodeId},
    templateId: {templateId},
    totalNum: {totalNum},
    pageCount: {pageCount},
    currentPageIndex: {currentPageIndex},
    stlPageContentsElement: '{TranslateUtils.EncryptStringBySecretKey(stlPageContentsElement)}'
}}";
        }
    }
}