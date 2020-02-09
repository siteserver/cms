using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/login")]
    public class PagesLoginController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var redirectUrl = await request.AdminRedirectCheckAsync(checkInstall: true, checkDatabaseVersion: true);
                if (!string.IsNullOrEmpty(redirectUrl)) return Ok(new
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                });

                var config = await DataProvider.ConfigRepository.GetAsync();

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