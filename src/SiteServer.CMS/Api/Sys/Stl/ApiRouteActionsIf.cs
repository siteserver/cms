using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsIf
    {
        public const string Route = "sys/stl/actions/if";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int siteId, int channelId, int contentId, int templateId, string ajaxDivId, string pageUrl, string testType, string testValue, string testOperate, string successTemplateString, string failureTemplateString)
        {
            return $@"{{
    siteId: {siteId},
    channelId: {channelId},
    contentId: {contentId},
    templateId: {templateId},
    ajaxDivId: '{ajaxDivId}',
    pageUrl: '{TranslateUtils.EncryptStringBySecretKey(pageUrl)}',
    testType: '{testType}',
    testValue: '{testValue}',
    testOperate: '{testOperate}',
    successTemplate: '{TranslateUtils.EncryptStringBySecretKey(successTemplateString)}',
    failureTemplate: '{TranslateUtils.EncryptStringBySecretKey(failureTemplateString)}'
}}";
        }
    }
}