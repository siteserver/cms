using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Json
{
    public class ApiRoutePluginJson
    {
        public const string Route = "json/{pluginId}";
        public const string RouteName = "json/{pluginId}/{name}";
        public const string RouteNameAndId = "json/{pluginId}/{name}/{id}";

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