using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages
{
    [OpenApiIgnore]
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
                    SystemManager.ProductVersion,
                    ConfigManager.SystemConfigInfo.AdminTitle
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}