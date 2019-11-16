using System;
using System.Threading.Tasks;
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
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                var redirect = await request.AdminRedirectCheckAsync(checkInstall: true, checkDatabaseVersion: true);
                if (redirect != null) return Ok(redirect);

                var config = await ConfigManager.GetInstanceAsync();

                return Ok(new
                {
                    Value = true,
                    SystemManager.ProductVersion,
                    config.AdminTitle
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}