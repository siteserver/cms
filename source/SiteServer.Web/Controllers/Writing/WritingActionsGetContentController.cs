using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsGetContentController : ApiController
    {
        [HttpPost, Route(ActionsGetContent.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var nodeId = body.GetPostInt("nodeId");
                var id = body.GetPostInt("id");

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, nodeId);

                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, id);

                if (contentInfo != null && contentInfo.WritingUserName == body.UserName)
                {
                    return Ok(new
                    {
                        Content =
                        ContentUtility.ContentToDictionary(contentInfo, tableStyle, tableName, relatedIdentities)
                    });
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
