using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Sys.Stl.Comments;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsAddController : ApiController
    {
        [HttpPost, Route(ActionsAdd.Route)]
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

                var account = context.GetPostString("account");
                var password = context.GetPostString("password");
                var replyId = context.GetPostInt("replyId");
                var content = context.GetPostString("content");

                if (replyId > 0)
                {
                    string replyUserName;
                    string replyContent;
                    DataProvider.CommentDao.GetUserNameAndContent(replyId, out replyUserName, out replyContent);
                    if (!string.IsNullOrEmpty(replyContent))
                    {
                        var displayName = BaiRongDataProvider.UserDao.GetDisplayName(replyUserName);
                        if (!string.IsNullOrEmpty(displayName))
                        {
                            displayName = $"@{displayName}：";
                        }

                        content += $" //{displayName}{replyContent}";
                    }
                }

                IUserInfo userInfo;
                if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
                {
                    string userName;
                    string errorMessage;
                    if (!BaiRongDataProvider.UserDao.Validate(account, password, out userName, out errorMessage))
                    {
                        BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                        return BadRequest(errorMessage);
                    }

                    BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userName);
                    userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(userName);

                    context.UserLogin(userName);
                }
                else
                {
                    userInfo = context.UserInfo;
                }

                if (!publishmentSystemInfo.Additional.IsAnonymousComments && !context.IsUserLoggin)
                {
                    return Unauthorized();
                }

                var commentInfo = new CommentInfo
                {
                    Id = 0,
                    PublishmentSystemId = siteId,
                    NodeId = channelId,
                    ContentId = contentId,
                    GoodCount = 0,
                    UserName = userInfo.UserName,
                    IsChecked = !publishmentSystemInfo.Additional.IsCheckComments,
                    AddDate = DateTime.Now,
                    Content = content
                };
                commentInfo.Id = DataProvider.CommentDao.Insert(commentInfo);

                return Ok(new
                {
                    Comment = new Comment(commentInfo, userInfo)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
