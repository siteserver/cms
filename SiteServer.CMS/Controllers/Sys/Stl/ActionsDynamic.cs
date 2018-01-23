using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsDynamic
    {
        public const string Route = "sys/stl/actions/dynamic";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int siteId, int pageNodeId, int pageContentId, int pageTemplateId, string pageUrl, string ajaxDivId, bool isPageRefresh, string templateContent)
        {
            return $@"{{
    siteId: {siteId},
    pageNodeId: {pageNodeId},
    pageContentId: {pageContentId},
    pageTemplateId: {pageTemplateId},
    isPageRefresh: {isPageRefresh.ToString().ToLower()},
    pageUrl: '{TranslateUtils.EncryptStringBySecretKey(pageUrl)}',
    ajaxDivId: '{ajaxDivId}',
    templateContent: '{TranslateUtils.EncryptStringBySecretKey(templateContent)}'
}}";
        }
    }
}