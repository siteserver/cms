using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Files
{
    public class UEditor
    {
        public const string Route = "files/ueditor/{publishmentSystemId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            return apiUrl;
        }
    }
}