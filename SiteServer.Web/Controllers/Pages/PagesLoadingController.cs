using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

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
                    Value = TranslateUtils.DecryptStringBySecretKey(redirectUrl, WebConfigUtils.SecretKey)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}