using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Packaging
{
    public class ApiRouteVersion
    {
        public const string Route = "sys/packaging/version/{packageId}";

        public static string GetUrl(string apiUrl, string packageId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{packageId}", packageId);
            return apiUrl;
        }
    }
}