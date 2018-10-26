using System.Text;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsRelatedFieldController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsRelatedField.Route)]
        public void Main(int siteId)
        {
            var request = new RequestImpl();

            var callback = request.GetQueryString("callback");
            var relatedFieldId = request.GetQueryInt("relatedFieldId");
            var parentId = request.GetQueryInt("parentId");
            var jsonString = GetRelatedField(relatedFieldId, parentId);
            var call = callback + "(" + jsonString + ")";

            HttpContext.Current.Response.Write(call);
            HttpContext.Current.Response.End();
        }

        public string GetRelatedField(int relatedFieldId, int parentId)
        {
            var jsonString = new StringBuilder();

            jsonString.Append("[");

            var list = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(relatedFieldId, parentId);
            if (list.Count > 0)
            {
                foreach (var itemInfo in list)
                {
                    jsonString.AppendFormat(@"{{""id"":""{0}"",""name"":""{1}"",""value"":""{2}""}},", itemInfo.Id, StringUtils.ToJsString(itemInfo.ItemName), StringUtils.ToJsString(itemInfo.ItemValue));
                }
                jsonString.Length -= 1;
            }

            jsonString.Append("]");
            return jsonString.ToString();
        }
    }
}
