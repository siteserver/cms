using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Packaging
{
    public class ApiRouteDownload
    {
        public const string Route = "sys/packaging/download";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}