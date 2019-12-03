using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Pages
{
    
    [RoutePrefix("pages/loading")]
    public class PagesLoadingController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var redirectUrl = request.GetPostString("redirectUrl");

                return Ok(new
                {
                    Value = WebConfigUtils.DecryptStringBySecretKey(redirectUrl)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}