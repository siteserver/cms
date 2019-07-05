using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/login")]
    public class PagesLoginController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var redirect = request.AdminRedirectCheck(checkInstall: true, checkDatabaseVersion: true);
                if (redirect != null) return Ok(redirect);

                return Ok(new
                {
                    Value = true,
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