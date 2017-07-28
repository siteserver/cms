using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsSendSmsController : ApiController
    {
        [HttpPost, Route(ActionsSendSms.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();
            var account = body.GetPostString("account");
            var mobile = BaiRongDataProvider.UserDao.GetMobileByAccount(account);

            var isSuccess = false;
            string errorMessage;
            if (string.IsNullOrEmpty(mobile) || !StringUtils.IsMobile(mobile))
            {
                errorMessage = "账号对应的用户未设置手机号码";
            }
            else
            {
                var code = StringUtils.GetRandomInt(1111, 9999);
                DbCacheManager.RemoveAndInsert($"SiteServer.API.Controllers.Users.SendSms.{mobile}.Code", code.ToString());
                isSuccess = SmsManager.SendCode(mobile, code, out errorMessage);
            }

            return Ok(new
            {
                IsSuccess = isSuccess,
                Mobile = mobile,
                ErrorMessage = errorMessage
            });
        }
    }
}
