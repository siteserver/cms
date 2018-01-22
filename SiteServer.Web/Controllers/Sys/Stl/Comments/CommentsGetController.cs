using System;
using System.Linq;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Sys.Stl.Comments;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsGetController : ApiController
    {
        [HttpGet, Route(Get.Route)]
        public IHttpActionResult Main(int siteId, int channelId, int contentId)
        {
            try
            {
                var context = new RequestContext();

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (!publishmentSystemInfo.Additional.IsCommentable)
                {
                    return Ok(new
                    {
                        IsCommentable = false
                    });
                }

                var requestCount = context.GetQueryInt("requestCount", 20);
                var requestOffset = context.GetQueryInt("requestOffset");

                var totalCount = DataProvider.CommentDao.GetTotalCountWithChecked(siteId, channelId, contentId);
                var comments =
                    DataProvider.CommentDao.GetCommentInfoListChecked(siteId, channelId, contentId, requestCount,
                            requestOffset)
                        .Select(
                            commentInfo =>
                                new Comment(commentInfo,
                                    DataProvider.UserDao.GetUserInfoByUserName(commentInfo.UserName)))
                        .ToList();

                return Ok(new
                {
                    IsCommentable = true,
                    User = context.UserInfo,
                    Comments = comments,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
