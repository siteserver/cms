using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Packaging
{
    public class ApiRouteUpdateSsCms
    {
        public const string Route = "sys/packaging/update/sscms";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }
    }
}