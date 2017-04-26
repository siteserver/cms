using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsVoteAdd
    {
        public const string Route = "stl/actions/vote_add/{publishmentSystemId}/{nodeId}/{contentId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int nodeId, int contentId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{nodeId}", nodeId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }
    }
}