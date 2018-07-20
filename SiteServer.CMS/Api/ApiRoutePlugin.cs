using SiteServer.Utils;

namespace SiteServer.CMS.Api
{
    public static class ApiRoutePlugin
    {
        public const string Route = "plugins/{pluginId}";
        public const string RouteAction = "plugins/{pluginId}/actions/{routeAction}";
        public const string RouteResource = "plugins/{pluginId}/{routeResource}";
        public const string RouteResourceAction = "plugins/{pluginId}/{routeResource}/actions/{routeAction}";
        public const string RouteResourceId = "plugins/{pluginId}/{routeResource}/{routeId}";
        public const string RouteResourceIdAction = "plugins/{pluginId}/{routeResource}/{routeId}/actions/{routeAction}";

        public static string GetRoute(string routeResource, string routeId, string routeAction)
        {
            var route = string.Empty;
            if (!string.IsNullOrEmpty(routeResource))
            {
                route = PageUtils.Combine(route, routeResource);
                if (!string.IsNullOrEmpty(routeId))
                {
                    route = PageUtils.Combine(route, routeId);
                    if (!string.IsNullOrEmpty(routeAction))
                    {
                        route = PageUtils.Combine(route, "actions", routeAction);
                    }
                }
                else if (!string.IsNullOrEmpty(routeAction))
                {
                    route = PageUtils.Combine(route, "actions", routeAction);
                }
            }
            else if (!string.IsNullOrEmpty(routeAction))
            {
                route = PageUtils.Combine(route, "actions", routeAction);
            }
            return route;
        }

        public static string GetUrl(string pluginId, string routeResource = "", string routeId = "", string routeAction = "")
        {
            var apiUrl = ApiManager.GetApiUrl(Route);
            apiUrl = apiUrl.Replace("{pluginId}", pluginId);
            if (!string.IsNullOrEmpty(routeResource))
            {
                apiUrl = PageUtils.Combine(apiUrl, routeResource);
                if (!string.IsNullOrEmpty(routeId))
                {
                    apiUrl = PageUtils.Combine(apiUrl, routeId);
                    if (!string.IsNullOrEmpty(routeAction))
                    {
                        apiUrl = PageUtils.Combine(apiUrl, "actions", routeAction);
                    }
                }
                else if (!string.IsNullOrEmpty(routeAction))
                {
                    apiUrl = PageUtils.Combine(apiUrl, "actions", routeAction);
                }
            }
            else if (!string.IsNullOrEmpty(routeAction))
            {
                apiUrl = PageUtils.Combine(apiUrl, "actions", routeAction);
            }
            return apiUrl;
        }
    }
}