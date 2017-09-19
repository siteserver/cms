using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PageApi : IPageApi
    {
        private readonly PluginMetadata _metadata;

        public PageApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public string GetPluginPageUrl(int publishmentSystemId, string relatedUrl = "")
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var apiUrl = PageUtility.GetOuterApiUrl(publishmentSystemInfo);
            return PageUtility.GetSiteFilesUrl(apiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public string GetPluginJsonApiUrl(int publishmentSystemId, string name = "", int id = 0)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var apiUrl = PageUtility.GetOuterApiUrl(publishmentSystemInfo);
            return Controllers.Plugins.JsonApi.GetUrl(apiUrl, _metadata.Id, name, id);
        }

        public string GetPluginHttpApiUrl(int publishmentSystemId, string name = "", int id = 0)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var apiUrl = PageUtility.GetOuterApiUrl(publishmentSystemInfo);
            return Controllers.Plugins.HttpApi.GetUrl(apiUrl, _metadata.Id, name, id);
        }

        public string FilterXss(string html)
        {
            return PageUtils.FilterXss(html);
        }

        public string GetHomeUrl(int publishmentSystemId, string relatedUrl = "")
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return HomeUtils.GetUrl(publishmentSystemInfo.Additional.HomeUrl, relatedUrl);
        }

        public string GetHomeLoginUrl(int publishmentSystemId, string returnUrl)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return HomeUtils.GetLoginUrl(publishmentSystemInfo.Additional.HomeUrl, returnUrl);
        }

        public string GetHomeLogoutUrl(int publishmentSystemId, string returnUrl)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return HomeUtils.GetLogoutUrl(publishmentSystemInfo.Additional.HomeUrl, returnUrl);
        }

        public string GetHomeRegisterUrl(int publishmentSystemId, string returnUrl)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return HomeUtils.GetRegisterUrl(publishmentSystemInfo.Additional.HomeUrl, returnUrl);
        }

        public string GetCurrentUrl(PluginParseContext context)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(context.PublishmentSystemId);
            return StlUtility.GetStlCurrentUrl(publishmentSystemInfo, context.ChannelId, context.ContentId,
                context.ContentInfo, ETemplateTypeUtils.GetEnumType(context.TemplateType), context.TemplateId);
        }

        public string GetPublishmentSystemUrl(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrl(publishmentSystemInfo, string.Empty, false);
        }

        public string GetPublishmentSystemUrlByFilePath(string filePath)
        {
            var publishmentSystemId = PublishmentSystemApi.Instance.GetPublishmentSystemIdByFilePath(filePath);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, filePath);
        }

        public string GetRootUrl(string relatedUrl)
        {
            return PageUtils.GetRootUrl(relatedUrl);
        }

        public string GetChannelUrl(int publishmentSystemId, int channelId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetChannelUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemId, channelId), false);
        }

        public string GetContentUrl(int publishmentSystemId, int channelId, int contentId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetContentUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemId, channelId), contentId, false);
        }
    }
}
