using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [OpenApiIgnore]
    [RoutePrefix("pages/loading")]
    public class PagesLoadingController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var redirectUrl = request.GetPostString("redirectUrl");

                return Ok(new
                {
                    Value = TranslateUtils.DecryptStringBySecretKey(redirectUrl)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}