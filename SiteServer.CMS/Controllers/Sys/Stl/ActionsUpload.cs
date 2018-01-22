using System.Collections.Specialized;
using SiteServer.Utils;

namespace SiteServer.CMS.Controllers.Sys.Stl
{
    public class ActionsUpload
    {
        public const string Route = "sys/stl/actions/upload/{publishmentSystemId}";

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