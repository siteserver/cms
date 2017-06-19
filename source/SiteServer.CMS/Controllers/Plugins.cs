using BaiRong.Core;

namespace SiteServer.CMS.Controllers
{
    public class Plugins
    {
        public const string Route = "plugins/{pluginId}";

        public static string GetUrl(string apiUrl, string pluginId, int siteId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{pluginId}", pluginId);
            return siteId > 0 ? apiUrl + "?siteId=" + siteId : apiUrl;
        }
    }
}