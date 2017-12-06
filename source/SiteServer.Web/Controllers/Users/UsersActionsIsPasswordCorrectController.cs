using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsIsPasswordCorrectController : ApiController
    {
        [HttpPost, Route(ActionsIsPasswordCorrect.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();
            var password = body.GetPostString("password");
            string errorMessage;
            var isCorrect = BaiRongDataProvider.UserDao.IsPasswordCorrect(password, out errorMessage);
            try
            {
                return Ok(new
                {
                    IsCorrect = isCorrect,
                    ErrorMessage = errorMessage
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
