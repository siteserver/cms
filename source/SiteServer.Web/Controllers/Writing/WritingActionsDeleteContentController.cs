using System;
using System.Collections.Generic;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsDeleteContentController : ApiController
    {
        [HttpPost, Route(ActionsDeleteContent.Route)]
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
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);

                var title = BaiRongDataProvider.ContentDao.GetValue(tableName, id, ContentAttribute.Title);

                var contentIdArrayList = new List<int> {id};
                DataProvider.ContentDao.TrashContents(publishmentSystemId, tableName, contentIdArrayList);

                LogUtils.AddUserLog(body.UserName, EUserActionType.WritingDelete, title);

                return Ok(new { });
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
