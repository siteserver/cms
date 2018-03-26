using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Packaging
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