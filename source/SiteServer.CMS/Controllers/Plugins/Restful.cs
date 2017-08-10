using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Plugins
{
    public class Restful
    {
        public const string Route = "plugins/restful/{pluginId}";
        public const string RouteName = "plugins/restful/{pluginId}/{name}";
        public const string RouteNameAndId = "plugins/restful/{pluginId}/{name}/{id}";

        public static string GetUrl(string apiUrl, string pluginId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{pluginId}", pluginId);
            return apiUrl;
        }
    }
}