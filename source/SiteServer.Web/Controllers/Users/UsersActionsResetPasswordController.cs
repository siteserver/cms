using System;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsResetPasswordController : ApiController
    {
        [HttpPost, Route(ActionsResetPassword.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();
            if (!body.IsUserLoggin) return Unauthorized();

            var password = body.GetPostString("password");
            var newPassword = body.GetPostString("newPassword");
            var confirmPassword = body.GetPostString("confirmPassword");

            string userName;
            string errorMessage;
            if (string.IsNullOrEmpty(password) || !BaiRongDataProvider.UserDao.ValidateAccount(body.UserName, password, out userName, out errorMessage))
            {
                return BadRequest("原密码输入错误，请重新输入");
            }
            if (password == newPassword)
            {
                return BadRequest("新密码不能与原密码一致，请重新输入");
            }

            if (BaiRongDataProvider.UserDao.ChangePassword(body.UserName, newPassword, out errorMessage))
            {
                LogUtils.AddUserLog(body.UserName, EUserActionType.UpdatePassword, string.Empty);

                return Ok(new
                {
                    LastResetPasswordDate = DateTime.Now
                });
            }

            return BadRequest(errorMessage);
        }
    }
}
