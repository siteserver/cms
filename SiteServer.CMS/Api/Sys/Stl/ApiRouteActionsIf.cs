using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public static class ApiRouteActionsIf
    {
        public const string Route = "sys/stl/actions/if";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}