using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsSendSmsOrRegisterController : ApiController
    {
        [HttpPost, Route(ActionsSendSmsOrRegister.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();
            var mobile = body.GetPostString("mobile");
            var password = body.GetPostString("password");

            var isSms = false;
            var isRegister = false;
            var errorMessage = string.Empty;

            if (ConfigManager.UserConfigInfo.RegisterVerifyType == EUserVerifyType.Mobile)
            {
                var code = StringUtils.GetRandomInt(1111, 9999);
                DbCacheManager.RemoveAndInsert($"SiteServer.API.Controllers.Users.SendSms.{mobile}.Code", code.ToString());
                isSms = SmsManager.SendCode(mobile, code, out errorMessage);
            }
            
            if (!isSms)
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
                IsSms = isSms,
                IsRegister = isRegister,
                ErrorMessage = errorMessage
            });
        }
    }
}
