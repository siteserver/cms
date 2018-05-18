using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using SiteServer.CMS.Api.V1;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("api")]
    public class PingController : ApiController
    {
        [HttpGet, Route(ApiPingRoute.Route)]
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent("pong", Encoding.UTF8);

            return response;
        }
    }
}
