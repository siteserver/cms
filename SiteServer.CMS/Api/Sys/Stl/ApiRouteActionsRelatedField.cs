using System.Collections.Specialized;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public class ApiRouteActionsRelatedField
    {
        public const string Route = "sys/stl/actions/related_field/{siteId}";

        public static string GetUrl(string apiUrl, int siteId, int relatedFieldId, int parentId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return PageUtils.AddQueryString(apiUrl, new NameValueCollection
            {
                {"relatedFieldId", relatedFieldId.ToString()},
                {"parentId", parentId.ToString()}
            });
        }
    }
}