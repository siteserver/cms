using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Packaging
{
    public class ApiRouteUpdateDatabase
    {
        public const string Route = "sys/packaging/update/database";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}