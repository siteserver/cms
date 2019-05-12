using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/error")]
    public class PagesErrorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest(HttpContext.Current.Request);
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var logId = request.GetQueryInt("logId");
                var logInfo = DataProvider.ErrorLogDao.GetErrorLogInfo(logId);
                if (logInfo == null) return NotFound();

                return Ok(new
                {
                    LogInfo = logInfo,
                    SystemManager.ProductVersion
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}