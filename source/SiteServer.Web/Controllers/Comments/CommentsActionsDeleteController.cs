using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Comments;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsDeleteController : ApiController
    {
        [HttpPost, Route(ActionsDelete.Route)]
        public IHttpActionResult Main(int siteId, int channelId, int contentId)
        {
            try
            {
                var body = new RequestBody();

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (!publishmentSystemInfo.Additional.IsCommentable)
                {
                    return Unauthorized();
                }

                var userName = body.GetPostString("userName");
                if (body.UserName != userName && !body.IsAdministratorLoggin)
                {
                    return Unauthorized();
                }

                DataProvider.CommentDao.Delete(siteId, channelId, contentId, body.GetPostInt("id"));

                return Ok();
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
