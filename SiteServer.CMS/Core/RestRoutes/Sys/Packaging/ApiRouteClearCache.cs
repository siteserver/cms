using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Packaging
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