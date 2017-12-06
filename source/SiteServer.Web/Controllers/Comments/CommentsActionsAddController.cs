using System;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Comments;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.API.Controllers.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsAddController : ApiController
    {
        [HttpPost, Route(ActionsAdd.Route)]
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

                var account = body.GetPostString("account");
                var password = body.GetPostString("password");
                var replyId = body.GetPostInt("replyId");
                var content = body.GetPostString("content");

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

                UserInfo userInfo;
                if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
                {
                    string userName;
                    string errorMessage;
                    if (!BaiRongDataProvider.UserDao.ValidateAccount(account, password, out userName, out errorMessage))
                    {
                        LogUtils.AddUserLog(userName, EUserActionType.LoginFailed, "用户登录失败");
                        BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                        return BadRequest(errorMessage);
                    }

                    BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userName);
                    userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(userName);

                    body.UserLogin(userName);
                }
                else
                {
                    userInfo = body.UserInfo;
                }

                if (!publishmentSystemInfo.Additional.IsAnonymousComments && !body.IsUserLoggin)
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
                    User = new User(userInfo),
                    Comment = new Comment(commentInfo, userInfo)
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
