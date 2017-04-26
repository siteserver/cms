using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsIf
    {
        public const string Route = "stl/actions/if";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(int publishmentSystemId, int channelId, int contentId, int templateId, string ajaxDivId, string pageUrl, string testType, string testValue, string testOperate, string successTemplateString, string failureTemplateString)
        {
            return $@"{{
    publishmentSystemId: {publishmentSystemId},
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