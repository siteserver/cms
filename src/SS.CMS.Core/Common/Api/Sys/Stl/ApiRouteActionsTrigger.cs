using System.Collections.Specialized;
using SS.CMS.Utils;

namespace SS.CMS.Core.Api.Sys.Stl
{
    public class ApiRouteActionsTrigger
    {
        public const string Route = "sys/stl/actions/trigger";

        public static string GetUrl(int siteId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(Constants.ApiPrefix, Route), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileTemplateId", fileTemplateId.ToString()},
                {"isRedirect", isRedirect.ToString()}
            });
        }
    }
}