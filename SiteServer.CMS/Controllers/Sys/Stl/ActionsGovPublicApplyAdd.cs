using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsGovPublicApplyAdd
    {
        public const string Route = "sys/stl/actions/gov_public_apply_add/{siteId}/{styleId}";

        public static string GetUrl(string apiUrl, int siteId, int styleId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{styleId}", styleId.ToString());
            return apiUrl;
        }
    }
}