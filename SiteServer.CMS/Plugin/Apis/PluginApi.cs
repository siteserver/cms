using System;
using System.Web;
using SiteServer.CMS.Api;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PluginApi : IPluginApi
    {
        private readonly IMetadata _metadata;

        public PluginApi(IMetadata metadata)
        {
            _metadata = metadata;
        }

        public string GetPluginUrl(string relatedUrl = "")
        {
            if (string.IsNullOrEmpty(relatedUrl)) return string.Empty;

            if (PageUtils.IsProtocolUrl(relatedUrl)) return relatedUrl;

            if (StringUtils.StartsWith(relatedUrl, "~/"))
            {
                return PageUtils.GetRootUrl(relatedUrl.Substring(1));
            }

            if (StringUtils.StartsWith(relatedUrl, "@/"))
            {
                return PageUtils.GetAdminDirectoryUrl(relatedUrl.Substring(1));
            }

            return PageUtility.GetSiteFilesUrl(ApiManager.ApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public string PluginApiUrl => ApiManager.GetApiUrl($"plugins/{_metadata.Id}");

        public string GetPluginPath(string relatedPath = "")
        {
            var path = PathUtils.Combine(PathUtils.GetPluginPath(_metadata.Id), relatedPath);
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public T GetPlugin<T>(string pluginId) where T : PluginBase
        {
            var pluginInfo = PluginManager.GetPluginInfo(pluginId);
            return pluginInfo?.Plugin as T;
        }
    }
}
