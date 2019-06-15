using SS.CMS.Utils;

namespace SS.CMS.Core.Api.Sys.Packaging
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