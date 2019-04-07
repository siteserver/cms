using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

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
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var logId = Request.GetQueryInt("logId");
                var logInfo = DataProvider.ErrorLog.GetErrorLogInfo(logId);
                if (logInfo == null) return NotFound();
                var retVal = logInfo.ToDictionary();

                return Ok(new
                {
                    LogInfo = logInfo,
                    SystemManager.Version
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}