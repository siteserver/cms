using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Api.Sys.Errors;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys
{
    public class SysErrorController : ControllerBase
    {
        [HttpGet, Route(ApiRouteError.Route)]
        public IHttpActionResult Main(int id)
        {
            return Ok(new
            {
                LogInfo = DataProvider.ErrorLogDao.GetErrorLogInfo(id),
                Version = SystemManager.ProductVersion
            });
        }
    }
}
