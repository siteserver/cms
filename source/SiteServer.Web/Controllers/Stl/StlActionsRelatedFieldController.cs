using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using System.Text;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsRelatedFieldController : ApiController
    {
        [HttpPost, Route(ActionsRelatedField.Route)]
        public void Main(int publishmentSystemId)
        {
            var body = new RequestBody();

            var callback = body.GetQueryString("callback");
            var relatedFieldId = body.GetQueryInt("relatedFieldId");
            var parentId = body.GetQueryInt("parentId");
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
                    jsonString.AppendFormat(@"{{""id"":""{0}"",""name"":""{1}"",""value"":""{2}""}},", itemInfo.ID, StringUtils.ToJsString(itemInfo.ItemName), StringUtils.ToJsString(itemInfo.ItemValue));
                }
                jsonString.Length -= 1;
            }

            jsonString.Append("]");
            return jsonString.ToString();
        }
    }
}
