using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsInputAdd
    {
        public const string Route = "sys/stl/actions/input_add/{publishmentSystemId}/{inputId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int inputId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{inputId}", inputId.ToString());
            return apiUrl;
        }
    }
}