using System;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/error")]
    public class PagesErrorController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = GetRequest();
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