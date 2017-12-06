using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Users;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsLoadConfigController : ApiController
    {
        [HttpPost, Route(ActionsLoadConfig.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                return Ok(ConfigManager.UserConfigInfo);
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex); 
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
