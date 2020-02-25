using System.Threading.Tasks;
using SiteServer.CMS.Api;
using SS.CMS;

using SS.CMS.Abstractions;
using SiteServer.CMS.Repositories;
using SS.CMS.Plugins;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PluginApi
    {
        private PluginApi() { }

        private static PluginApi _instance;
        public static PluginApi Instance => _instance ??= new PluginApi();

        public async Task<string> GetPluginUrlAsync(string pluginId, string relatedUrl = "")
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

            var config = await DataProvider.ConfigRepository.GetAsync();

            return PageUtility.GetSiteFilesUrl(config.GetApiUrl(), PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, pluginId, relatedUrl));
        }

        public async Task<string> GetPluginApiUrlAsync(string pluginId)
        {
            return await ApiManager.GetApiUrlAsync($"plugins/{pluginId}");
        }

        public string GetPluginPath(string pluginId, string relatedPath = "")
        {
            var path = PathUtils.Combine(WebUtils.GetPluginPath(pluginId), relatedPath);
            DirectoryUtils.CreateDirectoryIfNotExists(path);
            return path;
        }

        public async Task<T> GetPluginAsync<T>() where T : PluginBase
        {
            var pluginInfo = await PluginManager.GetPluginInfoAsync<T>();
            return pluginInfo?.Plugin as T;
        }
    }
}
