using System.Web.Http;
using SiteServer.CMS.Api.Sys.Errors;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys
{
    public class SysErrorController : ApiController
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
