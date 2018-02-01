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
            return PageUtility.GetSiteFilesUrl(PageUtility.OuterApiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public string GetPluginApiUrl(string action = "", string id = "")
        {
            return Controllers.ApiRoutePlugin.GetUrl(PageUtility.OuterApiUrl, _metadata.Id, action, id);
        }

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
