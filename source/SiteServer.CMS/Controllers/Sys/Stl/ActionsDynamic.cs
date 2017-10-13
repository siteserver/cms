using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsDynamic
    {
        public const string Route = "sys/stl/actions/dynamic";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int publishmentSystemId, int pageNodeId, int pageContentId, int pageTemplateId, string pageUrl, string ajaxDivId, bool isPageRefresh, string templateContent)
        {
            return $@"{{
    publishmentSystemId: {publishmentSystemId},
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