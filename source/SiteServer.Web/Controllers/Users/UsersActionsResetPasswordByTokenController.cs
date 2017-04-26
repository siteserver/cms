using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsResetPasswordByTokenController : ApiController
    {
        [HttpPost, Route(ActionsResetPasswordByToken.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();

            var token = body.GetPostString("token");
            var password = body.GetPostString("password");

            var userToken = RequestBody.GetUserToken(token);
            if (string.IsNullOrEmpty(userToken?.UserName))
            {
                return Unauthorized();
            }

            string errorMessage;
            var isSuccess = BaiRongDataProvider.UserDao.ChangePassword(userToken.UserName, password, out errorMessage);

            return Ok(new {
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage
            });
        }
    }
}
