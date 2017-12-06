using System;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsLoginController : ApiController
    {
        [HttpPost, Route(ActionsLogin.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                var account = body.GetPostString("account");
                var password = body.GetPostString("password");

                string userName;
                string errorMessage;
                if (!BaiRongDataProvider.UserDao.ValidateAccount(account, password, out userName, out errorMessage))
                {
                    LogUtils.AddUserLog(userName, EUserActionType.LoginFailed,  "用户登录失败");
                    BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                    return BadRequest(errorMessage);
                }

                BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userName);
                var userInfo = BaiRongDataProvider.UserDao.GetUserInfoByUserName(userName);
                var user = new User(userInfo);
                var groupInfo = UserGroupManager.GetGroupInfo(user.GroupId);

                body.UserLogin(userName);

                return Ok(new
                {
                    User = user,
                    Group = groupInfo.Additional
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
