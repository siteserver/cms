using System;
using System.Linq;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Comments;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Comments
{
    [RoutePrefix("api")]
    public class CommentsGetController : ApiController
    {
        [HttpGet, Route(Get.Route)]
        public IHttpActionResult Main(int siteId, int channelId, int contentId)
        {
            try
            {
                var body = new RequestBody();

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                if (!publishmentSystemInfo.Additional.IsCommentable)
                {
                    return Ok(new
                    {
                        IsCommentable = false
                    });
                }

                var requestCount = body.GetQueryInt("requestCount", 20);
                var requestOffset = body.GetQueryInt("requestOffset");

                var totalCount = DataProvider.CommentDao.GetTotalCountWithChecked(siteId, channelId, contentId);
                var comments =
                    DataProvider.CommentDao.GetCommentInfoListChecked(siteId, channelId, contentId, requestCount,
                            requestOffset)
                        .Select(
                            commentInfo =>
                                new Comment(commentInfo,
                                    BaiRongDataProvider.UserDao.GetUserInfoByUserName(commentInfo.UserName)))
                        .ToList();

                var user = new User(body.UserInfo);

                return Ok(new
                {
                    IsCommentable = true,
                    User = user,
                    Comments = comments,
                    TotalCount = totalCount
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
