using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

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
                var redirect = await request.AdminRedirectCheckAsync(checkInstall: true, checkDatabaseVersion: true);
                if (redirect != null) return Ok(redirect);

                var config = await DataProvider.ConfigDao.GetAsync();

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