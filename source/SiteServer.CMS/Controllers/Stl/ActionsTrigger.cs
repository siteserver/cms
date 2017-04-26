using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsTrigger
    {
        public const string Route = "stl/actions/trigger";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileTemplateId", fileTemplateId.ToString()},
                {"isRedirect", isRedirect.ToString()}
            });
        }
    }
}