using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Packaging
{
    public class ApiRouteUpdateSystem
    {
        public const string Route = "sys/packaging/update/system/{version}";

        public static string GetUrl(string apiUrl, string version)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{version}", version);
            return apiUrl;
        }
    }
}