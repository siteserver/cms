using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsResumeAdd
    {
        public const string Route = "sys/stl/actions/resume_add/{publishmentSystemId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            return apiUrl;
        }
    }
}