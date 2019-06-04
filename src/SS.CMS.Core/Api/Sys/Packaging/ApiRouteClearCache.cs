using SS.CMS.Utils;

namespace SS.CMS.Core.Api.Sys.Packaging
{
    public class ApiRouteClearCache
    {
        public const string Route = "sys/packaging/clear/cache";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}