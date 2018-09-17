using System.Web.Http;
using SiteServer.CMS.Api.Sys.Errors;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Sys.Errors
{
    [RoutePrefix("api")]
    public class ErrorController : ApiController
    {
        [HttpGet, Route(ApiRouteError.Route)]
        public IHttpActionResult Main(int id)
        {
            return Ok(new
            {
                LogInfo = DataProvider.ErrorLogDao.GetErrorLogInfo(id),
                SystemManager.Version
            });
        }
    }
}
