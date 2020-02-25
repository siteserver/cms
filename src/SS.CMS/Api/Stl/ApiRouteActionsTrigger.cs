using System.Collections.Specialized;
using SS.CMS.Core;

namespace SS.CMS.Api.Stl
{
    public static class ApiRouteActionsTrigger
    {
        public const string Route = "sys/stl/actions/trigger";

        public static string GetUrl(string apiUrl, int siteId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Route), new NameValueCollection
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