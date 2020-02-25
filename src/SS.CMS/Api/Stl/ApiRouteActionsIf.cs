using SS.CMS.Core;

namespace SS.CMS.Api.Stl
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