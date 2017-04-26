using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsRelatedField
    {
        public const string Route = "stl/actions/related_field/{publishmentSystemId}";

        public static string GetUrl(string apiUrl, int publishmentSystemId, int relatedFieldId, int parentId)
        {
            apiUrl = PageUtils.Combine(apiUrl, Route);
            apiUrl = apiUrl.Replace("{publishmentSystemId}", publishmentSystemId.ToString());
            return PageUtils.AddQueryString(apiUrl, new NameValueCollection
            {
                {"relatedFieldId", relatedFieldId.ToString()},
                {"parentId", parentId.ToString()}
            });
        }
    }
}