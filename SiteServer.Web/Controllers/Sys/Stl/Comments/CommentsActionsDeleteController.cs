using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Stl.Comments;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;

namespace SiteServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsDeleteController : ApiController
    {
        [HttpPost, Route(ActionsDelete.Route)]
        public IHttpActionResult Main(int siteId, int channelId, int contentId)
        {
            try
            {
                var context = new RequestContext();

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (!publishmentSystemInfo.Additional.IsCommentable)
                {
                    return Unauthorized();
                }

                var userName = context.GetPostString("userName");
                if (context.UserName != userName && !context.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                DataProvider.CommentDao.Delete(siteId, channelId, contentId, context.GetPostInt("id"));

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
