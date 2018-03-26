using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Packaging
{
    public class ApiRouteSyncDatabase
    {
        public const string Route = "sys/packaging/sync/database";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}