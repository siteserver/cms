using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Http
{
    public class ApiRoutePluginHttp
    {
        public const string Route = "http/{pluginId}";
        public const string RouteName = "http/{pluginId}/{name}";
        public const string RouteNameAndId = "http/{pluginId}/{name}/{id}";

        public static string GetUrl(string apiUrl, string pluginId, string name = "", string id = "")
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{pluginId}", pluginId);
            if (!string.IsNullOrEmpty(name))
            {
                apiUrl = PageUtils.Combine(apiUrl, name);
                if (!string.IsNullOrEmpty(id))
                {
                    apiUrl = PageUtils.Combine(apiUrl, id);
                }
            }
            return apiUrl;
        }
    }
}