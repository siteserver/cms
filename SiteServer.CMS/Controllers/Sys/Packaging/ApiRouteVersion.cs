using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Packaging
{
    public class ApiRouteVersion
    {
        public const string RouteCms = "sys/packaging/version/cms";
        public const string RoutePlugins = "sys/packaging/version/plugins";

        public static string GetCmsUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, RouteCms);
        }

        public static string GetPluginsUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, RoutePlugins);
        }
    }
}