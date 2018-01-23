using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsGovInteractApplyAdd
    {
        public const string Route = "sys/stl/actions/gov_interact_apply_add/{siteId}/{nodeId}/{styleId}";

        public static string GetUrl(string apiUrl, int siteId, int nodeId, int styleId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{nodeId}", nodeId.ToString());
            apiUrl = apiUrl.Replace("{styleId}", styleId.ToString());
            return apiUrl;
        }
    }
}