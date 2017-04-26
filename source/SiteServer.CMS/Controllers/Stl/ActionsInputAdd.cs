using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsInputAdd
    {
        public const string Route = "stl/actions/input_add/{publishmentSystemId}/{inputId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int inputId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            apiUrl = apiUrl.Replace("{inputId}", inputId.ToString());
            return apiUrl;
        }
    }
}