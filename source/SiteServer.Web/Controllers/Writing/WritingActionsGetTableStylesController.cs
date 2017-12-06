using System;
using System.Web.Http;
using BaiRong.Core.AuxiliaryTable;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsGetTableStylesController : ApiController
    {
        [HttpPost, Route(ActionsGetTableStyles.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var nodeId = body.GetPostInt("nodeId");

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, nodeId);

                return Ok(TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities));
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
