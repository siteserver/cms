using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Editors
{
    public class ApiRouteUEditor
    {
        public const string Route = "sys/editors/ueditor/{siteId}";

        public static string GetUrl(string apiUrl, int siteId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return apiUrl;
        }
    }
}