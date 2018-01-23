using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsVoteAdd
    {
        public const string Route = "sys/stl/actions/vote_add/{siteId}/{nodeId}/{contentId}";

        public static string GetUrl(string apiUrl, int siteId, int nodeId, int contentId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{nodeId}", nodeId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }
    }
}