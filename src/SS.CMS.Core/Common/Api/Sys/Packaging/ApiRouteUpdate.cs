using SS.CMS.Utils;

namespace SS.CMS.Core.Api.Sys.Packaging
{
    public class ApiRouteUpdate
    {
        public const string Route = "sys/packaging/update";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}