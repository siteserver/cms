using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsLogoutController : ApiController
    {
        [HttpPost, Route(ActionsLogout.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();

            try
            {
                body.UserLogout();

                return Ok();
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
