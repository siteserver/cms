using SS.CMS.Abstractions;
using SS.CMS.Core.Api;
using SS.CMS.Core.Common;
using SS.CMS.Core.Settings;
using SS.CMS.Utils;

namespace SS.CMS.Core.Plugin.Apis
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
                return AppContext.GetRootUrl(relatedUrl.Substring(1));
            }

            if (StringUtils.StartsWith(relatedUrl, "@/"))
            {
                return AppContext.GetAdminUrl(relatedUrl.Substring(1));
            }

            return PageUtility.GetSiteFilesUrl(ApiManager.ApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, pluginId, relatedUrl));
        }

        public string GetPluginApiUrl(string pluginId)
        {
            return ApiManager.GetApiUrl($"plugins/{pluginId}");
        }

        public string GetPluginPath(string pluginId, string relatedPath = "")
        {
            var path = PathUtils.Combine(PathUtilsEx.GetPluginPath(pluginId), relatedPath);
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
