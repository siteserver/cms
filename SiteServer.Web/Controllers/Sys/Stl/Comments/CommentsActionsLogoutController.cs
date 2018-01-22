using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Stl.Comments;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.API.Controllers.Sys.Stl.Comments
{
    [RoutePrefix("api")]
    public class CommentsActionsLogoutController : ApiController
    {
        [HttpPost, Route(ActionsLogout.Route)]
        public IHttpActionResult Main()
        {
            var context = new RequestContext();

            try
            {
                context.UserLogout();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
