using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsGovInteractQueryAdd
    {
        public const string Route = "stl/actions/gov_interact_query_add/{publishmentSystemId}/{nodeId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int nodeId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{nodeId}", nodeId.ToString());
            return apiUrl;
        }
    }
}