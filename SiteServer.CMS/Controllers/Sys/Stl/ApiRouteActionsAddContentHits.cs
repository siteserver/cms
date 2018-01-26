using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ApiRouteActionsAddContentHits
    {
        public const string Route = "sys/stl/actions/add_content_hits/{siteId}/{channelId}/{contentId}";

        public static string GetUrl(string apiUrl, int siteId, int channelId, int contentId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }
    }
}