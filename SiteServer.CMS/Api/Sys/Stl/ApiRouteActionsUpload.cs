using System.Collections.Specialized;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsUpload
    {
        public const string Route = "sys/stl/actions/upload/{siteId}";

        public const string TypeResume = "Resume";
        public const string TypeGovPublicApply = "GovPublicApply";

        public static string GetUrl(string apiUrl, int siteId, string type)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return PageUtils.AddQueryString(apiUrl, new NameValueCollection
            {
                {"type", type }
            });
        }
    }
}