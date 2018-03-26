using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsDynamic
    {
        public const string Route = "sys/stl/actions/dynamic";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int siteId, int pageChannelId, int pageContentId, int pageTemplateId, string pageUrl, string ajaxDivId, bool isPageRefresh, string templateContent)
        {
            return $@"{{
    siteId: {siteId},
    pageChannelId: {pageChannelId},
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