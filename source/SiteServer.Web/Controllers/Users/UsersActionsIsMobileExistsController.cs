using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsIsMobileExistsController : ApiController
    {
        [HttpPost, Route(ActionsIsMobileExists.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();
            var mobile = body.GetPostString("mobile");
            try
            {
                return Ok(new
                {
                    Exists = BaiRongDataProvider.UserDao.IsMobileExists(mobile)
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
