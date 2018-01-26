using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.API.Controllers.Sys.Packaging
{
    [RoutePrefix("api")]
    public class PackagesUpdateSystemController : ApiController
    {
        [HttpGet, Route(ApiRouteUpdateSystem.Route)]
        public IHttpActionResult Main(string version)
        {
            var context = new RequestContext();

            if (!context.IsAdminLoggin)
            {
                return Unauthorized();
            }

            string errorMessage;
            var isSuccess = SystemManager.UpdateSystem(version, out errorMessage);

            return Ok(new
            {
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage
            });
        }
    }
}
