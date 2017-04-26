using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsUpload
    {
        public const string Route = "stl/actions/upload/{publishmentSystemId}";

        public const string TypeResume = "Resume";
        public const string TypeGovPublicApply = "GovPublicApply";

        public static string GetUrl(string apiUrl, int publishmentSystemId, string type)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            return PageUtils.AddQueryString(apiUrl, new NameValueCollection
            {
                {"type", type }
            });
        }
    }
}