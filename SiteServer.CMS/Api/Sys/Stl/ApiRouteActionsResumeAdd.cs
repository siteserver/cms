using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsResumeAdd
    {
        public const string Route = "sys/stl/actions/resume_add/{siteId}";

        public static string GetUrl(string apiUrl, int siteId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return apiUrl;
        }
    }
}