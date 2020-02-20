using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Sys
{
    
    public class SysStlActionsRelatedFieldController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsRelatedField.Route)]
        public async Task Main(int siteId)
        {
            var request = await AuthenticatedRequest.GetAuthAsync();

            var callback = request.GetQueryString("callback");
            var relatedFieldId = request.GetQueryInt("relatedFieldId");
            var parentId = request.GetQueryInt("parentId");
            var jsonString = await GetRelatedFieldAsync(siteId, relatedFieldId, parentId);
            var call = callback + "(" + jsonString + ")";

            HttpContext.Current.Response.Write(call);
            HttpContext.Current.Response.End();
        }

        private async Task<string> GetRelatedFieldAsync(int siteId, int relatedFieldId, int parentId)
        {
            var jsonString = new StringBuilder();

            jsonString.Append("[");

            var list = await DataProvider.RelatedFieldItemRepository.GetListAsync(siteId, relatedFieldId, parentId);
            if (list.Any())
            {
                foreach (var itemInfo in list)
                {
                    jsonString.AppendFormat(@"{{""id"":""{0}"",""name"":""{1}"",""value"":""{2}""}},", itemInfo.Id, StringUtils.ToJsString(itemInfo.Label), StringUtils.ToJsString(itemInfo.Value));
                }
                jsonString.Length -= 1;
            }

            jsonString.Append("]");
            return jsonString.ToString();
        }
    }
}
