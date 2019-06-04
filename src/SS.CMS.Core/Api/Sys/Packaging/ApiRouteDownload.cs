using SS.CMS.Utils;

namespace SS.CMS.Core.Api.Sys.Packaging
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