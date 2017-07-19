using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Plugins
{
    public class Root
    {
        public const string Route = "plugins/{pluginId}";

        public static string GetUrl(string apiUrl, string pluginId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{pluginId}", pluginId);
            return apiUrl;
        }
    }
}