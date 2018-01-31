using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Packaging
{
    public class ApiRouteDownload
    {
        public const string Route = "sys/packaging/download/{packageId}/{version}";

        public static string GetUrl(string apiUrl, string packageId, string version)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{packageId}", packageId);
            apiUrl = apiUrl.Replace("{version}", version);
            return apiUrl;
        }
    }
}