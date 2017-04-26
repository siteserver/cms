using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsVote
    {
        public const string Route = "stl/actions/vote/{publishmentSystemId}/{channelId}/{contentId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int channelId, int contentId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }
    }
}