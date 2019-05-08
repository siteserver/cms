using System;
using System.Web.Http;
using SiteServer.API.Controllers.Backend;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

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
                var rest = Request.GetAuthenticatedRequest();
                var redirect = LoginController.AdminRedirectCheck(rest, checkInstall: true, checkDatabaseVersion: true);
                if (redirect != null) return Ok(redirect);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}