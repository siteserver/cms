using SiteServer.CMS.Api;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PluginApi : IPluginApi
    {
        private PluginApi() { }

        private static PluginApi _instance;
        public static PluginApi Instance => _instance ?? (_instance = new PluginApi());

        public string GetPluginUrl(string pluginId, string relatedUrl = "")
        {
            if (PageUtils.IsProtocolUrl(relatedUrl)) return relatedUrl;

            if (StringUtils.StartsWith(relatedUrl, "~/"))
            {
                return PageUtils.GetRootUrl(relatedUrl.Substring(1));
            }

            if (StringUtils.StartsWith(relatedUrl, "@/"))
            {
                return PageUtils.GetAdminUrl(relatedUrl.Substring(1));
            }

            return PageUtility.GetSiteFilesUrl(ApiManager.ApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, pluginId, relatedUrl));
        }

        public string GetPluginApiUrl(string pluginId)
        {
            return ApiManager.GetApiUrl($"plugins/{pluginId}");
        }

        public string GetPluginPath(string pluginId, string relatedPath = "")
        {
            var path = PathUtils.Combine(PathUtils.GetPluginPath(pluginId), relatedPath);
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public T GetPlugin<T>() where T : PluginBase
        {
            var pluginInfo = PluginManager.GetPluginInfo<T>();
            return pluginInfo?.Plugin as T;
        }
    }
}
