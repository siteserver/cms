using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsIsCodeCorrectController : ApiController
    {
        [HttpPost, Route(ActionsIsCodeCorrect.Route)]
        public IHttpActionResult IsCodeCorrect()
        {
            var body = new RequestBody();
            var mobile = body.GetPostString("mobile");
            var code = body.GetPostString("code");

            var dbCode = DbCacheManager.GetValue($"SiteServer.API.Controllers.Users.SendSms.{mobile}.Code");

            var isCorrect = code == dbCode;
            var token = string.Empty;
            if (isCorrect)
            {
                token = RequestBody.GetUserTokenStr(BaiRongDataProvider.UserDao.GetUserNameByMobile(mobile));
            }

            return Ok(new
            {
                IsCorrect = isCorrect,
                Token = token
            });
        }
    }
}
