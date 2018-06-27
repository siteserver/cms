using SiteServer.Utils;

namespace SiteServer.CMS.Api
{
    public class ApiRoutePlugin
    {
        public const string Route = "plugins/{pluginId}";
        public const string RouteAction = "plugins/{pluginId}/actions/{act}";
        public const string RouteResource = "plugins/{pluginId}/{resource}";
        public const string RouteResourceAction = "plugins/{pluginId}/{resource}/actions/{act}";
        public const string RouteResourceId = "plugins/{pluginId}/{resource}/{id}";
        public const string RouteResourceIdAction = "plugins/{pluginId}/{resource}/{id}/actions/{act}";

        public static string GetUrl(string pluginId, string resource = "", string id = "", string action = "")
        {
            var apiUrl = ApiManager.GetApiUrl(Route);
            apiUrl = apiUrl.Replace("{pluginId}", pluginId);
            if (!string.IsNullOrEmpty(resource))
            {
                apiUrl = PageUtils.Combine(apiUrl, resource);
                if (!string.IsNullOrEmpty(id))
                {
                    apiUrl = PageUtils.Combine(apiUrl, id);
                    if (!string.IsNullOrEmpty(action))
                    {
                        apiUrl = PageUtils.Combine(apiUrl, "actions", action);
                    }
                }
                else if (!string.IsNullOrEmpty(action))
                {
                    apiUrl = PageUtils.Combine(apiUrl, "actions", action);
                }
            }
            else if (!string.IsNullOrEmpty(action))
            {
                apiUrl = PageUtils.Combine(apiUrl, "actions", action);
            }
            return apiUrl;
        }
    }
}