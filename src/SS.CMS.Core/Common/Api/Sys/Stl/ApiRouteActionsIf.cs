using SS.CMS.Utils;

namespace SS.CMS.Core.Api.Sys.Stl
{
    public static class ApiRouteActionsIf
    {
        public const string Route = "sys/stl/actions/if";

        public static string GetUrl()
        {
            return PageUtils.Combine(Constants.ApiPrefix, Route);
        }
    }
}