using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsRegisterWithCodeController : ApiController
    {
        [HttpPost, Route(ActionsRegisterWithCode.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();

            var mobile = body.GetPostString("mobile");
            var password = body.GetPostString("password");
            var code = body.GetPostString("code");

            var dbCode = DbCacheManager.GetValue($"SiteServer.API.Controllers.Users.SendSms.{mobile}.Code");

            var isRegister = false;
            string errorMessage;

            if (code != dbCode)
            {
                errorMessage = "短信验证码不正确";
            }
            else
            {
                var userInfo = new UserInfo
                {
                    UserName = mobile,
                    Mobile = mobile,
                    Password = password
                };
                isRegister = BaiRongDataProvider.UserDao.Insert(userInfo, PageUtils.GetIpAddress(), out errorMessage);
            }

            return Ok(new {
                IsRegister = isRegister,
                ErrorMessage = errorMessage
            });
        }
    }
}
