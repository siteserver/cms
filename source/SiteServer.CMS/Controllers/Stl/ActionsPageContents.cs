using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsPageContents
    {
        public const string Route = "stl/actions/pagecontents";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int publishmentSystemId, int pageNodeId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement)
        {
            return $@"
{{
    publishmentSystemId: {publishmentSystemId},
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