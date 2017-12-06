using System;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsCreateContentController : ApiController
    {
        [HttpPost, Route(ActionsCreateContent.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var nodeId = body.GetPostInt("nodeId");

                var user = new User(body.UserInfo);
                var groupInfo = UserGroupManager.GetGroupInfo(user.GroupId);
                var adminUserName = groupInfo.Additional.WritingAdminUserName;

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, nodeId);

                var contentInfo = ContentUtility.GetContentInfo(tableStyle);

                var postCollection = body.GetPostCollection();

                InputTypeParser.AddValuesToAttributes(tableStyle, tableName, publishmentSystemInfo, relatedIdentities, postCollection, contentInfo.Attributes, ContentAttribute.HiddenAttributes);

                contentInfo.IsChecked = false;
                contentInfo.PublishmentSystemId = publishmentSystemId;
                contentInfo.NodeId = nodeId;
                contentInfo.AddUserName = adminUserName;
                contentInfo.WritingUserName = user.UserName;
                contentInfo.LastEditUserName = adminUserName;
                contentInfo.AddDate = DateTime.Now;
                contentInfo.LastEditDate = DateTime.Now;

                var contentId = DataProvider.ContentDao.Insert(tableName, publishmentSystemInfo, contentInfo);

                LogUtils.AddUserLog(body.UserName, EUserActionType.WritingAdd, contentInfo.Title);

                return Ok(new
                {
                    ID = contentId
                });
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
