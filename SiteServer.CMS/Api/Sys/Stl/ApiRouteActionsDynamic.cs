using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public static class ApiRouteActionsDynamic
    {
        public const string Route = "sys/stl/actions/dynamic";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}