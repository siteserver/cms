using BaiRong.Core;
using SiteServer.CMS.Core;
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
    }
}
