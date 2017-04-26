using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsGovInteractApplyAdd
    {
        public const string Route = "stl/actions/gov_interact_apply_add/{publishmentSystemId}/{nodeId}/{styleId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int nodeId, int styleId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{nodeId}", nodeId.ToString());
            apiUrl = apiUrl.Replace("{styleId}", styleId.ToString());
            return apiUrl;
        }
    }
}