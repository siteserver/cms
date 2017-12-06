using System;
using System.Collections.Generic;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsGetChannelsController : ApiController
    {
        [HttpPost, Route(ActionsGetChannels.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");

                var groupInfo = UserGroupManager.GetGroupInfo(body.UserInfo.GroupId);
                var adminUserName = groupInfo.Additional.WritingAdminUserName;

                var nodeInfoList = PublishmentSystemManager.GetWritingNodeInfoList(adminUserName, publishmentSystemId);
                var nodes = new List<object>();
                foreach (var nodeInfo in nodeInfoList)
                {
                    nodes.Add(new
                    {
                        nodeInfo.NodeId,
                        nodeInfo.NodeName
                    });
                }

                return Ok(nodes);
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
