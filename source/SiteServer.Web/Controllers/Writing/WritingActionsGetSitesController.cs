using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsGetSitesController : ApiController
    {
        [HttpPost, Route(ActionsGetSites.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var groupInfo = UserGroupManager.GetGroupInfo(body.UserInfo.GroupId);
                var adminUserName = groupInfo.Additional.WritingAdminUserName;

                var publishmentSystemInfoList = PublishmentSystemManager.GetWritingPublishmentSystemInfoList(adminUserName);

                return Ok(publishmentSystemInfoList);
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
