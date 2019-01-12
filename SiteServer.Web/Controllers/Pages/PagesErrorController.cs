using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;

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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var logId = request.GetQueryInt("logId");

                return Ok(new
                {
                    LogInfo = DataProvider.ErrorLogDao.GetErrorLogInfo(logId),
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