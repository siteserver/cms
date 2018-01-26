using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ApiRouteActionsGovInteractQueryAdd
    {
        public const string Route = "sys/stl/actions/gov_interact_query_add/{siteId}/{nodeId}";

        public static string GetUrl(string apiUrl, int siteId, int nodeId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{nodeId}", nodeId.ToString());
            return apiUrl;
        }
    }
}